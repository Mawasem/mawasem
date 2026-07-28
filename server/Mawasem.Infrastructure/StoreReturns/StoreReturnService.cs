using Mawasem.Application.Features.StoreReturns.Contracts.Requests;
using Mawasem.Application.Features.StoreReturns.Contracts.Responses;
using Mawasem.Application.Features.StoreReturns.Interfaces;
using Mawasem.Application.Features.StoreReturns.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;

namespace Mawasem.Infrastructure.StoreReturns;

public sealed class StoreReturnService : IStoreReturnService
{
    private readonly MawasemDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public StoreReturnService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<StoreReturnResult<StoreReturnResponse>> CreateAsync(
        int storeEmployeeId ,
        int orderId ,
        CreateStoreReturnRequest request ,
        CancellationToken cancellationToken = default )
    {
        if ( storeEmployeeId <= 0 || orderId <= 0 )
            return Failure(StoreReturnErrorCodes.InvalidRequest ,
                "A valid employee and POS order are required.");

        if ( request.Items.Count == 0 )
            return Failure(StoreReturnErrorCodes.ItemsRequired ,
                "At least one returned item is required.");

        if ( request.Items.GroupBy(item => item.OrderItemId)
            .Any(group => group.Count() > 1) )
            return Failure(StoreReturnErrorCodes.DuplicateOrderItem ,
                "An order item cannot appear more than once.");

        if ( request.Items.Any(item => item.OrderItemId <= 0 || item.Quantity <= 0) )
            return Failure(StoreReturnErrorCodes.InvalidQuantity ,
                "Every returned item requires a valid quantity.");

        if ( request.RefundPaymentMethod is not (
            PaymentMethod.CashAtStore or
            PaymentMethod.CardAtStore or
            PaymentMethod.InstaPayAtStore ) )
            return Failure(StoreReturnErrorCodes.InvalidPaymentMethod ,
                "Store returns must use cash, card, or InstaPay.");

        var reference = string.IsNullOrWhiteSpace(request.RefundPaymentReference)
            ? null
            : request.RefundPaymentReference.Trim();

        if ( request.RefundPaymentMethod is PaymentMethod.CardAtStore or PaymentMethod.InstaPayAtStore &&
            string.IsNullOrWhiteSpace(reference) )
            return Failure(StoreReturnErrorCodes.PaymentReferenceRequired ,
                "A payment reference is required for card and InstaPay refunds.");

        await using var transaction = await _dbContext.Database
            .BeginTransactionAsync(IsolationLevel.Serializable , cancellationToken);

        try
        {
            var order = await _dbContext.Orders
                .Include(candidate => candidate.OrderItems)
                    .ThenInclude(item => item.ProductVariant)
                .SingleOrDefaultAsync(
                    candidate => candidate.Id == orderId &&
                                 candidate.OrderSource == OrderSource.Store &&
                                 !candidate.IsDeleted ,
                    cancellationToken);

            if ( order is null )
            {
                await transaction.RollbackAsync(cancellationToken);
                return Failure(StoreReturnErrorCodes.OrderNotFound ,
                    "The POS receipt was not found.");
            }

            if ( order.OrderStatus is not OrderStatus.Delivered and not OrderStatus.PartiallyRefunded )
            {
                await transaction.RollbackAsync(cancellationToken);
                return Failure(StoreReturnErrorCodes.InvalidStoreOrder ,
                    "Only completed POS sales can be returned.");
            }

            var now = _timeProvider.GetUtcNow();
            decimal total = 0m;

            var storeReturn = new StoreReturn
            {
                OrderId = order.Id ,
                ReturnNumber = $"RET-{now:yyyyMMddHHmmss}-{Convert.ToHexString(RandomNumberGenerator.GetBytes(3))}" ,
                RefundPaymentMethod = request.RefundPaymentMethod ,
                RefundPaymentReference = reference ,
                ReturnedAtUtc = now.UtcDateTime ,
                ProcessedByEmployeeId = storeEmployeeId ,
                CreatedOn = now ,
                CreatedBy = $"DashboardUser:{storeEmployeeId}"
            };

            foreach ( var requestItem in request.Items )
            {
                var orderItem = order.OrderItems.SingleOrDefault(
                    item => item.Id == requestItem.OrderItemId &&
                            !item.IsDeleted);

                if ( orderItem is null )
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Failure(StoreReturnErrorCodes.OrderItemNotFound ,
                        "A returned item does not belong to the POS receipt.");
                }

                var remaining = orderItem.Quantity - orderItem.RefundedQuantity;

                if ( requestItem.Quantity > remaining )
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return Failure(StoreReturnErrorCodes.QuantityExceedsReturnable ,
                        "The returned quantity exceeds the quantity available for return.");
                }

                var amount = decimal.Round(
                    orderItem.UnitPrice * requestItem.Quantity ,
                    2 ,
                    MidpointRounding.AwayFromZero);

                orderItem.RefundedQuantity += requestItem.Quantity;
                orderItem.ProductVariant.StockQuantity += requestItem.Quantity;
                orderItem.ProductVariant.LastModifiedOn = now;
                orderItem.ProductVariant.LastModifiedBy = $"DashboardUser:{storeEmployeeId}";

                storeReturn.Items.Add(new StoreReturnItem
                {
                    OrderItemId = orderItem.Id ,
                    Quantity = requestItem.Quantity ,
                    UnitRefundAmount = orderItem.UnitPrice ,
                    TotalRefundAmount = amount ,
                    Reason = string.IsNullOrWhiteSpace(requestItem.Reason)
                        ? null
                        : requestItem.Reason.Trim() ,
                    CreatedOn = now ,
                    CreatedBy = $"DashboardUser:{storeEmployeeId}"
                });

                total += amount;
            }

            storeReturn.TotalRefundAmount = total;

            order.OrderStatus = order.OrderItems.All(
                item => item.RefundedQuantity == item.Quantity)
                ? OrderStatus.Refunded
                : OrderStatus.PartiallyRefunded;

            order.PaymentStatus = order.OrderStatus == OrderStatus.Refunded
                ? PaymentStatus.Refunded
                : PaymentStatus.PartiallyRefunded;

            _dbContext.StoreReturns.Add(storeReturn);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return StoreReturnResult<StoreReturnResponse>.Success(
                new StoreReturnResponse
                {
                    StoreReturnId = storeReturn.Id ,
                    ReturnNumber = storeReturn.ReturnNumber ,
                    OrderId = order.Id ,
                    OrderNumber = order.OrderNumber ,
                    OrderStatus = order.OrderStatus ,
                    TotalRefundAmount = total ,
                    RefundPaymentMethod = storeReturn.RefundPaymentMethod ,
                    RefundPaymentReference = storeReturn.RefundPaymentReference ,
                    ReturnedAtUtc = storeReturn.ReturnedAtUtc
                });
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(cancellationToken);
            return Failure(StoreReturnErrorCodes.ConcurrencyConflict ,
                "The POS order changed while the return was being processed.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(cancellationToken);
            return Failure(StoreReturnErrorCodes.OperationFailed ,
                "The store return could not be saved.");
        }
    }

    private static StoreReturnResult<StoreReturnResponse> Failure(
        string code ,
        string message ) =>
        StoreReturnResult<StoreReturnResponse>.Failure(code , message);
}