using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Contracts.Responses;
using Mawasem.Application.Features.StoreOrders.Interfaces;
using Mawasem.Application.Features.StoreOrders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Security.Cryptography;

namespace Mawasem.Infrastructure.StoreOrders;

public sealed class StoreOrderService : IStoreOrderService
{
    private const int MaximumIdempotencyKeyLength = 128;
    private const int MaximumPaymentReferenceLength = 200;
    private const int MaximumNotesLength = 1000;

    private readonly MawasemDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public StoreOrderService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<StoreOrderResult<StoreOrderReceiptResponse>>
        CreateAsync(
            int storeEmployeeId ,
            CreateStoreOrderRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( storeEmployeeId <= 0 )
            return Failure(StoreOrderErrorCodes.InvalidEmployee ,
                "The store employee identifier must be greater than zero.");

        var key = request.IdempotencyKey?.Trim();

        if ( string.IsNullOrWhiteSpace(key) ||
            key.Length > MaximumIdempotencyKeyLength )
            return Failure(StoreOrderErrorCodes.InvalidIdempotencyKey ,
                $"The idempotency key is required and cannot exceed {MaximumIdempotencyKeyLength} characters.");

        if ( request.Items.Count == 0 )
            return Failure(StoreOrderErrorCodes.ItemsRequired ,
                "At least one product variant is required.");

        if ( request.Items.Any(item =>
                item.ProductVariantId <= 0 || item.Quantity <= 0) )
            return Failure(StoreOrderErrorCodes.InvalidQuantity ,
                "Every item must contain a valid product variant and a quantity greater than zero.");

        if ( request.Items.GroupBy(item => item.ProductVariantId)
            .Any(group => group.Count() > 1) )
            return Failure(StoreOrderErrorCodes.DuplicateVariant ,
                "A product variant cannot appear more than once.");

        if ( request.PaymentMethod is not (
            PaymentMethod.CashAtStore or
            PaymentMethod.CardAtStore or
            PaymentMethod.InstaPayAtStore ) )
            return Failure(StoreOrderErrorCodes.InvalidPaymentMethod ,
                "Store sales must use cash, card, or InstaPay.");

        var reference = Normalize(request.PaymentReference);

        if ( reference?.Length > MaximumPaymentReferenceLength )
            return Failure(StoreOrderErrorCodes.InvalidPaymentReference ,
                $"The payment reference cannot exceed {MaximumPaymentReferenceLength} characters.");

        if ( request.PaymentMethod is PaymentMethod.CardAtStore or PaymentMethod.InstaPayAtStore &&
            string.IsNullOrWhiteSpace(reference) )
            return Failure(StoreOrderErrorCodes.PaymentReferenceRequired ,
                "A payment reference is required for card and InstaPay payments.");

        var notes = Normalize(request.Notes);

        if ( notes?.Length > MaximumNotesLength )
            return Failure(StoreOrderErrorCodes.InvalidNotes ,
                $"Notes cannot exceed {MaximumNotesLength} characters.");

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable , cancellationToken);

        try
        {
            var employee = await _dbContext.Users.SingleOrDefaultAsync(
                user => user.Id == storeEmployeeId , cancellationToken);

            if ( employee is null )
            {
                await transaction.RollbackAsync(cancellationToken);
                return Failure(StoreOrderErrorCodes.StoreEmployeeNotFound ,
                    "The store employee was not found.");
            }

            if ( employee.IsBlocked )
            {
                await transaction.RollbackAsync(cancellationToken);
                return Failure(StoreOrderErrorCodes.StoreEmployeeBlocked ,
                    "The store employee is blocked.");
            }

            var existingOrder = await _dbContext.Orders
                .Include(order => order.OrderItems)
                .Include(order => order.StatusHistory)
                .SingleOrDefaultAsync(
                    order => order.OrderSource == OrderSource.Store &&
                             order.UserId == null &&
                             order.IdempotencyKey == key &&
                             !order.IsDeleted ,
                    cancellationToken);

            if ( existingOrder is not null )
            {
                await transaction.CommitAsync(cancellationToken);

                return StoreOrderResult<StoreOrderReceiptResponse>.Success(
                    CreateReceiptResponse(existingOrder));
            }

            var variantIds = request.Items
                .Select(item => item.ProductVariantId)
                .ToArray();

            var variants = await _dbContext.ProductVariants
                .Include(variant => variant.Product)
                .Include(variant => variant.Options)
                    .ThenInclude(option => option.ProductOptionValue)
                        .ThenInclude(value => value.ProductOption)
                .Where(variant => variantIds.Contains(variant.Id))
                .ToDictionaryAsync(variant => variant.Id , cancellationToken);

            if ( variants.Count != variantIds.Length )
            {
                await transaction.RollbackAsync(cancellationToken);
                return Failure(StoreOrderErrorCodes.InvalidVariant ,
                    "One or more product variants were not found.");
            }

            var now = _timeProvider.GetUtcNow();
            var actor = $"DashboardUser:{storeEmployeeId.ToString(CultureInfo.InvariantCulture)}";

            var order = new Order
            {
                CustomerNameAr = "عميل المتجر" ,
                CustomerNameEn = "Walk-in Customer" ,
                CustomerPhone = string.Empty ,
                OrderNumber = CreateOrderNumber(now) ,
                OrderDate = now.UtcDateTime ,
                IdempotencyKey = key ,
                PaymentMethod = request.PaymentMethod ,
                PaymentStatus = PaymentStatus.Paid ,
                PaymentReference = reference ,
                PaidAtUtc = now.UtcDateTime ,
                DeliveryMethod = DeliveryMethod.StorePickup ,
                OrderSource = OrderSource.Store ,
                OrderStatus = OrderStatus.Delivered ,
                DeliveryFee = 0m ,
                Notes = notes ,
                CreatedOn = now ,
                CreatedBy = actor
            };

            decimal total = 0m;

            foreach ( var requestItem in request.Items )
            {
                var variant = variants[requestItem.ProductVariantId];
                var product = variant.Product;

                if ( product.IsDeleted || !product.IsPublished ||
                    variant.IsDeleted || !variant.IsAvailable )
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Failure(StoreOrderErrorCodes.VariantUnavailable ,
                        $"Product variant '{variant.SKU}' is not available for sale.");
                }

                if ( variant.StockQuantity < requestItem.Quantity )
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Failure(StoreOrderErrorCodes.InsufficientStock ,
                        $"Only {variant.StockQuantity} unit(s) of '{variant.SKU}' are currently available.");
                }

                var lineTotal = RoundMoney(product.CurrentPrice * requestItem.Quantity);
                total = RoundMoney(total + lineTotal);

                var summaries = CreateVariantSummaries(variant);

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id ,
                    ProductVariantId = variant.Id ,
                    ProductNameAr = product.Name.Arabic ,
                    ProductNameEn = product.Name.English ,
                    SKU = variant.SKU ,
                    VariantSummaryAr = summaries.Arabic ,
                    VariantSummaryEn = summaries.English ,
                    UnitPrice = product.CurrentPrice ,
                    DiscountAmount = 0m ,
                    Quantity = requestItem.Quantity ,
                    TotalPrice = lineTotal ,
                    RefundedQuantity = 0 ,
                    CreatedOn = now ,
                    CreatedBy = actor
                });

                variant.StockQuantity -= requestItem.Quantity;
                variant.LastModifiedOn = now;
                variant.LastModifiedBy = actor;
            }

            order.SubTotal = total;
            order.Discount = 0m;
            order.TotalAmount = total;

            order.StatusHistory.Add(new OrderStatusHistory
            {
                PreviousStatus = OrderStatus.Pending ,
                NewStatus = OrderStatus.Delivered ,
                ChangedByUserId = storeEmployeeId ,
                ActorType = OrderStatusChangeActorType.DashboardUser ,
                ChangedAtUtc = now.UtcDateTime ,
                Reason = "Walk-in store sale completed."
            });

            _dbContext.Orders.Add(order);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return StoreOrderResult<StoreOrderReceiptResponse>.Success(
                CreateReceiptResponse(order));
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();

            return Failure(StoreOrderErrorCodes.ConcurrencyConflict ,
                "Stock changed while the store sale was being created. Please try again.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();

            return Failure(StoreOrderErrorCodes.OperationFailed ,
                "The store sale could not be saved. Please try again.");
        }
    }

    public async Task<StoreOrderResult<StoreOrderReceiptResponse>>
        GetReceiptAsync(
            int orderId ,
            CancellationToken cancellationToken = default )
    {
        if ( orderId <= 0 )
            return Failure(StoreOrderErrorCodes.InvalidRequest ,
                "The order identifier must be greater than zero.");

        var order = await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.OrderItems)
            .Include(order => order.StatusHistory)
            .SingleOrDefaultAsync(
                order => order.Id == orderId &&
                         order.OrderSource == OrderSource.Store &&
                         !order.IsDeleted ,
                cancellationToken);

        if ( order is null )
            return Failure(StoreOrderErrorCodes.OrderNotFound ,
                "The store receipt was not found.");

        return StoreOrderResult<StoreOrderReceiptResponse>.Success(
            CreateReceiptResponse(order));
    }

    private static StoreOrderReceiptResponse CreateReceiptResponse( Order order )
    {
        var employeeId = order.StatusHistory
            .OrderByDescending(history => history.Id)
            .Select(history => history.ChangedByUserId)
            .FirstOrDefault() ?? 0;

        return new StoreOrderReceiptResponse
        {
            OrderId = order.Id ,
            ReceiptNumber = order.OrderNumber ,
            OrderDate = order.OrderDate ,
            PaymentMethod = order.PaymentMethod ,
            PaymentStatus = order.PaymentStatus ,
            PaymentReference = order.PaymentReference ,
            PaidAtUtc = order.PaidAtUtc ,
            SubTotal = order.SubTotal ,
            Discount = order.Discount ,
            TotalAmount = order.TotalAmount ,
            ProcessedByEmployeeId = employeeId ,
            Items = order.OrderItems
                .Where(item => !item.IsDeleted)
                .OrderBy(item => item.Id)
                .Select(item => new StoreOrderReceiptItemResponse
                {
                    OrderItemId = item.Id ,
                    ProductId = item.ProductId ,
                    ProductVariantId = item.ProductVariantId ,
                    ProductNameAr = item.ProductNameAr ,
                    ProductNameEn = item.ProductNameEn ,
                    Sku = item.SKU ,
                    VariantSummaryAr = item.VariantSummaryAr ,
                    VariantSummaryEn = item.VariantSummaryEn ,
                    UnitPrice = item.UnitPrice ,
                    DiscountAmount = item.DiscountAmount ,
                    Quantity = item.Quantity ,
                    LineTotal = item.TotalPrice
                })
                .ToArray()
        };
    }

    private static StoreOrderResult<StoreOrderReceiptResponse> Failure(
        string code ,
        string message )
    {
        return StoreOrderResult<StoreOrderReceiptResponse>.Failure(
            code , message);
    }

    private static string? Normalize( string? value )
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static decimal RoundMoney( decimal value )
    {
        return decimal.Round(value , 2 , MidpointRounding.AwayFromZero);
    }

    private static string CreateOrderNumber( DateTimeOffset now )
    {
        var suffix = Convert.ToHexString(RandomNumberGenerator.GetBytes(3));
        return $"MWS-{now:yyyyMMddHHmmss}-{suffix}";
    }

    private static (string Arabic , string English) CreateVariantSummaries(
        ProductVariant variant )
    {
        var options = variant.Options
            .Where(option =>
                !option.IsDeleted &&
                !option.ProductOptionValue.IsDeleted &&
                !option.ProductOptionValue.ProductOption.IsDeleted)
            .OrderBy(option => option.ProductOptionValue.ProductOptionId)
            .ThenBy(option => option.ProductOptionValueId)
            .ToArray();

        return (
            string.Join("، " , options.Select(option =>
                $"{option.ProductOptionValue.ProductOption.Name.Arabic}: {option.ProductOptionValue.Value.Arabic}")) ,
            string.Join(", " , options.Select(option =>
                $"{option.ProductOptionValue.ProductOption.Name.English}: {option.ProductOptionValue.Value.English}")));
    }
}