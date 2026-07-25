using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Orders;
using Mawasem.Tests.Checkout;

namespace Mawasem.Tests.Orders;

public sealed class AdminOrderDetailsQueryTests
{
    private static readonly DateTime OrderDateUtc =
        new(
            2026 ,
            7 ,
            25 ,
            14 ,
            0 ,
            0 ,
            DateTimeKind.Utc);

    [Fact]
    public async Task GetAdminDetailsAsync_ActiveOrder_ReturnsCompleteSnapshots()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        var orderId =
            await SeedOrderAsync(
                database ,
                OrderStatus.Pending);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetAdminDetailsAsync(
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        var response =
            result.Response;

        Assert.Equal(
            orderId ,
            response.Id);

        Assert.Equal(
            "MWS-ADMIN-DETAILS" ,
            response.OrderNumber);

        Assert.Equal(
            OrderDateUtc ,
            response.OrderDate);

        Assert.Equal(
            OrderStatus.Pending ,
            response.OrderStatus);

        Assert.Equal(
            PaymentMethod.CashOnDelivery ,
            response.PaymentMethod);

        Assert.Equal(
            PaymentStatus.Pending ,
            response.PaymentStatus);

        Assert.Equal(
            DeliveryMethod.HomeDelivery ,
            response.DeliveryMethod);

        Assert.Equal(
            OrderSource.Website ,
            response.OrderSource);

        Assert.Equal(
            400m ,
            response.SubTotal);

        Assert.Equal(
            40m ,
            response.Discount);

        Assert.Equal(
            50m ,
            response.DeliveryFee);

        Assert.Equal(
            410m ,
            response.TotalAmount);

        Assert.Equal(
            "ADMIN-COUPON" ,
            response.CouponCode);

        Assert.Equal(
            "Admin order details notes" ,
            response.Notes);

        Assert.Equal(
            "admin-details-idempotency-key" ,
            response.IdempotencyKey);

        Assert.Null(
            response.StockRestoredAtUtc);

        Assert.Equal(
            CheckoutTestDatabase.CustomerId ,
            response.Customer.UserId);

        Assert.Equal(
            "اسم العميل المحفوظ" ,
            response.Customer.NameAr);

        Assert.Equal(
            "Saved Customer Name" ,
            response.Customer.NameEn);

        Assert.Equal(
            "+201000000021" ,
            response.Customer.Phone);

        Assert.Equal(
            CheckoutTestDatabase.AddressId ,
            response.Shipping.SourceAddressId);

        Assert.Equal(
            CheckoutTestDatabase.DeliveryAreaId ,
            response.Shipping.DeliveryAreaId);

        Assert.Equal(
            "منطقة الشحن المحفوظة" ,
            response.Shipping.DeliveryAreaNameAr);

        Assert.Equal(
            "Saved Shipping Area" ,
            response.Shipping.DeliveryAreaNameEn);

        Assert.Equal(
            "Saved Recipient" ,
            response.Shipping.RecipientName);

        Assert.Equal(
            "+201000000022" ,
            response.Shipping.RecipientPhone);

        Assert.Equal(
            "Cairo" ,
            response.Shipping.City);

        Assert.Equal(
            "Nasr City" ,
            response.Shipping.AreaName);

        Assert.Equal(
            "20 Saved Address Street" ,
            response.Shipping.DetailedAddress);

        Assert.Equal(
            "20" ,
            response.Shipping.BuildingNumber);

        Assert.Equal(
            "4" ,
            response.Shipping.FloorNumber);

        Assert.Equal(
            "12" ,
            response.Shipping.ApartmentNumber);

        Assert.Equal(
            "Near Saved Admin Landmark" ,
            response.Shipping.Landmark);

        Assert.Equal(
            2 ,
            response.DistinctItemCount);

        Assert.Equal(
            5 ,
            response.TotalQuantity);

        Assert.True(
            response.CanConfirm);

        Assert.True(
            response.CanReject);

        Assert.True(
            response.CanCancel);

        var items =
            response.Items.ToArray();

        Assert.Equal(
            2 ,
            items.Length);

        Assert.Equal(
            "Admin Saved Product One" ,
            items[0].ProductNameEn);

        Assert.Equal(
            "منتج إداري محفوظ أول" ,
            items[0].ProductNameAr);

        Assert.Equal(
            "ADMIN-DETAILS-101" ,
            items[0].Sku);

        Assert.Equal(
            "Blue / Medium" ,
            items[0].VariantSummaryEn);

        Assert.Equal(
            "أزرق / متوسط" ,
            items[0].VariantSummaryAr);

        Assert.Equal(
            120m ,
            items[0].UnitPrice);

        Assert.Equal(
            20m ,
            items[0].DiscountAmount);

        Assert.Equal(
            2 ,
            items[0].Quantity);

        Assert.Equal(
            200m ,
            items[0].LineTotal);

        Assert.Equal(
            0 ,
            items[0].RefundedQuantity);

        Assert.Equal(
            "Admin Saved Product Two" ,
            items[1].ProductNameEn);

        Assert.Equal(
            3 ,
            items[1].Quantity);

        Assert.Equal(
            210m ,
            items[1].LineTotal);

        Assert.DoesNotContain(
            items ,
            item =>
                item.Sku ==
                "ADMIN-DETAILS-DELETED");
    }

    [Fact]
    public async Task GetAdminDetailsAsync_ConfirmedOrder_ReturnsCorrectActions()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        var orderId =
            await SeedOrderAsync(
                database ,
                OrderStatus.Confirmed);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetAdminDetailsAsync(
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.False(
            result.Response.CanConfirm);

        Assert.False(
            result.Response.CanReject);

        Assert.True(
            result.Response.CanCancel);
    }

    [Fact]
    public async Task GetAdminDetailsAsync_CancelledOrder_ReturnsCancellationAndStockMetadata()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        var cancelledAtUtc =
            OrderDateUtc.AddHours(2);

        var stockRestoredAtUtc =
            OrderDateUtc.AddHours(3);

        var orderId =
            await SeedOrderAsync(
                database ,
                OrderStatus.Cancelled ,
                cancellationReason:
                    "Cancelled by dashboard administrator." ,
                cancelledAtUtc:
                    cancelledAtUtc ,
                stockRestoredAtUtc:
                    stockRestoredAtUtc);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetAdminDetailsAsync(
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            "Cancelled by dashboard administrator." ,
            result.Response.CancellationReason);

        Assert.Equal(
            cancelledAtUtc ,
            result.Response.CancelledAtUtc);

        Assert.Equal(
            stockRestoredAtUtc ,
            result.Response.StockRestoredAtUtc);

        Assert.False(
            result.Response.CanConfirm);

        Assert.False(
            result.Response.CanReject);

        Assert.False(
            result.Response.CanCancel);
    }

    [Fact]
    public async Task GetAdminDetailsAsync_RejectedOrder_ReturnsRejectionMetadata()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        var rejectedAtUtc =
            OrderDateUtc.AddHours(4);

        var stockRestoredAtUtc =
            OrderDateUtc.AddHours(5);

        var orderId =
            await SeedOrderAsync(
                database ,
                OrderStatus.Rejected ,
                rejectionReason:
                    "Order rejected because inventory was unavailable." ,
                rejectedAtUtc:
                    rejectedAtUtc ,
                stockRestoredAtUtc:
                    stockRestoredAtUtc);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetAdminDetailsAsync(
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            "Order rejected because inventory was unavailable." ,
            result.Response.RejectionReason);

        Assert.Equal(
            rejectedAtUtc ,
            result.Response.RejectedAtUtc);

        Assert.Equal(
            stockRestoredAtUtc ,
            result.Response.StockRestoredAtUtc);

        Assert.False(
            result.Response.CanConfirm);

        Assert.False(
            result.Response.CanReject);

        Assert.False(
            result.Response.CanCancel);
    }

    [Fact]
    public async Task GetAdminDetailsAsync_MissingOrder_ReturnsNotFound()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetAdminDetailsAsync(
                999999);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.OrderNotFound ,
            result.ErrorCode);

        Assert.Null(
            result.Response);
    }

    [Fact]
    public async Task GetAdminDetailsAsync_DeletedOrder_ReturnsNotFound()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        var orderId =
            await SeedOrderAsync(
                database ,
                OrderStatus.Pending ,
                isDeleted: true);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetAdminDetailsAsync(
                orderId);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.OrderNotFound ,
            result.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetAdminDetailsAsync_InvalidOrderId_ReturnsFailure(
        int orderId )
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetAdminDetailsAsync(
                orderId);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.InvalidRequest ,
            result.ErrorCode);
    }

    private static async Task<int> SeedOrderAsync(
        CheckoutTestDatabase database ,
        OrderStatus orderStatus ,
        string? cancellationReason = null ,
        DateTime? cancelledAtUtc = null ,
        string? rejectionReason = null ,
        DateTime? rejectedAtUtc = null ,
        DateTime? stockRestoredAtUtc = null ,
        bool isDeleted = false )
    {
        await using var dbContext =
            database.CreateContext();

        var order =
            new Order
            {
                UserId =
                    CheckoutTestDatabase.CustomerId ,

                CustomerNameAr =
                    "اسم العميل المحفوظ" ,

                CustomerNameEn =
                    "Saved Customer Name" ,

                CustomerPhone =
                    "+201000000021" ,

                UserAddressId =
                    CheckoutTestDatabase.AddressId ,

                ShippingDeliveryAreaId =
                    CheckoutTestDatabase.DeliveryAreaId ,

                ShippingRecipientName =
                    "Saved Recipient" ,

                ShippingRecipientPhone =
                    "+201000000022" ,

                ShippingCity =
                    "Cairo" ,

                ShippingAreaName =
                    "Nasr City" ,

                ShippingDetailedAddress =
                    "20 Saved Address Street" ,

                ShippingBuildingNumber =
                    "20" ,

                ShippingFloorNumber =
                    "4" ,

                ShippingApartmentNumber =
                    "12" ,

                ShippingLandmark =
                    "Near Saved Admin Landmark" ,

                ShippingDeliveryAreaNameAr =
                    "منطقة الشحن المحفوظة" ,

                ShippingDeliveryAreaNameEn =
                    "Saved Shipping Area" ,

                OrderNumber =
                    "MWS-ADMIN-DETAILS" ,

                OrderDate =
                    OrderDateUtc ,

                IdempotencyKey =
                    "admin-details-idempotency-key" ,

                SubTotal =
                    400m ,

                Discount =
                    40m ,

                DeliveryFee =
                    50m ,

                TotalAmount =
                    410m ,

                CouponCode =
                    "ADMIN-COUPON" ,

                OrderStatus =
                    orderStatus ,

                PaymentMethod =
                    PaymentMethod.CashOnDelivery ,

                PaymentStatus =
                    PaymentStatus.Pending ,

                DeliveryMethod =
                    DeliveryMethod.HomeDelivery ,

                OrderSource =
                    OrderSource.Website ,

                Notes =
                    "Admin order details notes" ,

                CancellationReason =
                    cancellationReason ,

                CancelledAtUtc =
                    cancelledAtUtc ,

                RejectionReason =
                    rejectionReason ,

                RejectedAtUtc =
                    rejectedAtUtc ,

                StockRestoredAtUtc =
                    stockRestoredAtUtc ,

                IsDeleted =
                    isDeleted ,

                CreatedOn =
                    new DateTimeOffset(
                        OrderDateUtc) ,

                CreatedBy =
                    "test"
            };

        order.OrderItems.Add(
            CreateOrderItem(
                productNameAr:
                    "منتج إداري محفوظ أول" ,

                productNameEn:
                    "Admin Saved Product One" ,

                sku:
                    "ADMIN-DETAILS-101" ,

                variantSummaryAr:
                    "أزرق / متوسط" ,

                variantSummaryEn:
                    "Blue / Medium" ,

                unitPrice:
                    120m ,

                discountAmount:
                    20m ,

                quantity:
                    2 ,

                lineTotal:
                    200m));

        order.OrderItems.Add(
            CreateOrderItem(
                productNameAr:
                    "منتج إداري محفوظ ثان" ,

                productNameEn:
                    "Admin Saved Product Two" ,

                sku:
                    "ADMIN-DETAILS-102" ,

                variantSummaryAr:
                    string.Empty ,

                variantSummaryEn:
                    string.Empty ,

                unitPrice:
                    70m ,

                discountAmount:
                    0m ,

                quantity:
                    3 ,

                lineTotal:
                    210m));

        order.OrderItems.Add(
            CreateOrderItem(
                productNameAr:
                    "منتج إداري محذوف" ,

                productNameEn:
                    "Deleted Admin Product" ,

                sku:
                    "ADMIN-DETAILS-DELETED" ,

                variantSummaryAr:
                    string.Empty ,

                variantSummaryEn:
                    string.Empty ,

                unitPrice:
                    500m ,

                discountAmount:
                    0m ,

                quantity:
                    10 ,

                lineTotal:
                    5000m ,

                isDeleted:
                    true));

        dbContext.Orders.Add(order);

        await dbContext.SaveChangesAsync();

        return order.Id;
    }

    private static OrderItem CreateOrderItem(
        string productNameAr ,
        string productNameEn ,
        string sku ,
        string variantSummaryAr ,
        string variantSummaryEn ,
        decimal unitPrice ,
        decimal discountAmount ,
        int quantity ,
        decimal lineTotal ,
        bool isDeleted = false )
    {
        return new OrderItem
        {
            ProductId =
                CheckoutTestDatabase.ProductId ,

            ProductVariantId =
                CheckoutTestDatabase.ProductVariantId ,

            ProductNameAr =
                productNameAr ,

            ProductNameEn =
                productNameEn ,

            SKU =
                sku ,

            VariantSummaryAr =
                variantSummaryAr ,

            VariantSummaryEn =
                variantSummaryEn ,

            UnitPrice =
                unitPrice ,

            DiscountAmount =
                discountAmount ,

            Quantity =
                quantity ,

            TotalPrice =
                lineTotal ,

            RefundedQuantity =
                0 ,

            IsDeleted =
                isDeleted ,

            CreatedOn =
                DateTimeOffset.UtcNow ,

            CreatedBy =
                "test"
        };
    }
}