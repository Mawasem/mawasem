using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Orders;
using Mawasem.Tests.Checkout;

namespace Mawasem.Tests.Orders;

public sealed class CustomerOrderDetailsQueryTests
{
    private static readonly DateTime OrderDateUtc =
        new(
            2026 ,
            7 ,
            25 ,
            10 ,
            0 ,
            0 ,
            DateTimeKind.Utc);

    [Fact]
    public async Task GetCustomerDetailsAsync_OwnOrder_ReturnsImmutableSnapshots()
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
                CheckoutTestDatabase.CustomerId ,
                OrderStatus.Confirmed);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        var response =
            result.Response;

        Assert.Equal(
            orderId ,
            response.Id);

        Assert.Equal(
            "MWS-DETAILS-TEST" ,
            response.OrderNumber);

        Assert.Equal(
            OrderDateUtc ,
            response.OrderDate);

        Assert.Equal(
            OrderStatus.Confirmed ,
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
            300m ,
            response.SubTotal);

        Assert.Equal(
            20m ,
            response.Discount);

        Assert.Equal(
            50m ,
            response.DeliveryFee);

        Assert.Equal(
            330m ,
            response.TotalAmount);

        Assert.Equal(
            "TEST-COUPON" ,
            response.CouponCode);

        Assert.Equal(
            "Customer notes snapshot" ,
            response.Notes);

        Assert.False(
            response.CanCancel);

        Assert.Equal(
            2 ,
            response.DistinctItemCount);

        Assert.Equal(
            5 ,
            response.TotalQuantity);

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
            "+201000000001" ,
            response.Shipping.RecipientPhone);

        Assert.Equal(
            "Cairo" ,
            response.Shipping.City);

        Assert.Equal(
            "Nasr City" ,
            response.Shipping.AreaName);

        Assert.Equal(
            "10 Saved Address Street" ,
            response.Shipping.DetailedAddress);

        Assert.Equal(
            "10" ,
            response.Shipping.BuildingNumber);

        Assert.Equal(
            "3" ,
            response.Shipping.FloorNumber);

        Assert.Equal(
            "8" ,
            response.Shipping.ApartmentNumber);

        Assert.Equal(
            "Near Saved Landmark" ,
            response.Shipping.Landmark);

        var items =
            response.Items.ToArray();

        Assert.Equal(
            2 ,
            items.Length);

        Assert.Equal(
            "Saved Product One" ,
            items[0].ProductNameEn);

        Assert.Equal(
            "منتج محفوظ أول" ,
            items[0].ProductNameAr);

        Assert.Equal(
            "TEST-101" ,
            items[0].Sku);

        Assert.Equal(
            "Red / Large" ,
            items[0].VariantSummaryEn);

        Assert.Equal(
            "أحمر / كبير" ,
            items[0].VariantSummaryAr);

        Assert.Equal(
            100m ,
            items[0].UnitPrice);

        Assert.Equal(
            10m ,
            items[0].DiscountAmount);

        Assert.Equal(
            2 ,
            items[0].Quantity);

        Assert.Equal(
            180m ,
            items[0].LineTotal);

        Assert.Equal(
            0 ,
            items[0].RefundedQuantity);

        Assert.Equal(
            "Saved Product Two" ,
            items[1].ProductNameEn);

        Assert.Equal(
            3 ,
            items[1].Quantity);

        Assert.Equal(
            150m ,
            items[1].LineTotal);
    }

    [Fact]
    public async Task GetCustomerDetailsAsync_PendingOrder_CanBeCancelled()
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
                CheckoutTestDatabase.CustomerId ,
                OrderStatus.Pending);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.True(
            result.Response.CanCancel);
    }

    [Fact]
    public async Task GetCustomerDetailsAsync_CancelledOrder_ReturnsCancellationMetadata()
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

        var orderId =
            await SeedOrderAsync(
                database ,
                CheckoutTestDatabase.CustomerId ,
                OrderStatus.Cancelled ,
                cancellationReason:
                    "Customer cancelled the order." ,
                cancelledAtUtc:
                    cancelledAtUtc);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            "Customer cancelled the order." ,
            result.Response.CancellationReason);

        Assert.Equal(
            cancelledAtUtc ,
            result.Response.CancelledAtUtc);

        Assert.False(
            result.Response.CanCancel);
    }

    [Fact]
    public async Task GetCustomerDetailsAsync_RejectedOrder_ReturnsRejectionMetadata()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        var rejectedAtUtc =
            OrderDateUtc.AddHours(3);

        var orderId =
            await SeedOrderAsync(
                database ,
                CheckoutTestDatabase.CustomerId ,
                OrderStatus.Rejected ,
                rejectionReason:
                    "The order could not be fulfilled." ,
                rejectedAtUtc:
                    rejectedAtUtc);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            "The order could not be fulfilled." ,
            result.Response.RejectionReason);

        Assert.Equal(
            rejectedAtUtc ,
            result.Response.RejectedAtUtc);

        Assert.False(
            result.Response.CanCancel);
    }

    [Fact]
    public async Task GetCustomerDetailsAsync_OtherCustomersOrder_ReturnsNotFound()
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
                CheckoutTestDatabase.OtherCustomerId ,
                OrderStatus.Pending);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                orderId);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.OrderNotFound ,
            result.ErrorCode);

        Assert.Null(result.Response);
    }

    [Fact]
    public async Task GetCustomerDetailsAsync_DeletedOrder_ReturnsNotFound()
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
                CheckoutTestDatabase.CustomerId ,
                OrderStatus.Pending ,
                isDeleted: true);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                orderId);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.OrderNotFound ,
            result.ErrorCode);
    }

    [Fact]
    public async Task GetCustomerDetailsAsync_BlockedCustomer_ReturnsFailure()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CustomerBlocked = true ,
                CreateCart = false
            });

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                1);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.CustomerBlocked ,
            result.ErrorCode);
    }

    [Fact]
    public async Task GetCustomerDetailsAsync_MissingCustomer_ReturnsFailure()
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
            await service.GetCustomerDetailsAsync(
                999999 ,
                1);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.CustomerNotFound ,
            result.ErrorCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetCustomerDetailsAsync_InvalidOrderId_ReturnsFailure(
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
            await service.GetCustomerDetailsAsync(
                CheckoutTestDatabase.CustomerId ,
                orderId);

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.InvalidRequest ,
            result.ErrorCode);
    }

    private static async Task<int> SeedOrderAsync(
        CheckoutTestDatabase database ,
        int userId ,
        OrderStatus orderStatus ,
        string? cancellationReason = null ,
        DateTime? cancelledAtUtc = null ,
        string? rejectionReason = null ,
        DateTime? rejectedAtUtc = null ,
        bool isDeleted = false )
    {
        await using var dbContext =
            database.CreateContext();

        var order =
            new Order
            {
                UserId =
                    userId ,

                CustomerNameAr =
                    "اسم العميل المحفوظ" ,

                CustomerNameEn =
                    "Saved Customer Name" ,

                CustomerPhone =
                    "+201000000001" ,

                UserAddressId =
                    CheckoutTestDatabase.AddressId ,

                ShippingDeliveryAreaId =
                    CheckoutTestDatabase.DeliveryAreaId ,

                ShippingRecipientName =
                    "Saved Recipient" ,

                ShippingRecipientPhone =
                    "+201000000001" ,

                ShippingCity =
                    "Cairo" ,

                ShippingAreaName =
                    "Nasr City" ,

                ShippingDetailedAddress =
                    "10 Saved Address Street" ,

                ShippingBuildingNumber =
                    "10" ,

                ShippingFloorNumber =
                    "3" ,

                ShippingApartmentNumber =
                    "8" ,

                ShippingLandmark =
                    "Near Saved Landmark" ,

                ShippingDeliveryAreaNameAr =
                    "منطقة الشحن المحفوظة" ,

                ShippingDeliveryAreaNameEn =
                    "Saved Shipping Area" ,

                OrderNumber =
                    "MWS-DETAILS-TEST" ,

                OrderDate =
                    OrderDateUtc ,

                SubTotal =
                    300m ,

                Discount =
                    20m ,

                DeliveryFee =
                    50m ,

                TotalAmount =
                    330m ,

                CouponCode =
                    "TEST-COUPON" ,

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
                    "Customer notes snapshot" ,

                CancellationReason =
                    cancellationReason ,

                CancelledAtUtc =
                    cancelledAtUtc ,

                RejectionReason =
                    rejectionReason ,

                RejectedAtUtc =
                    rejectedAtUtc ,

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
                    "منتج محفوظ أول" ,

                productNameEn:
                    "Saved Product One" ,

                sku:
                    "TEST-101" ,

                variantSummaryAr:
                    "أحمر / كبير" ,

                variantSummaryEn:
                    "Red / Large" ,

                unitPrice:
                    100m ,

                discountAmount:
                    10m ,

                quantity:
                    2 ,

                lineTotal:
                    180m));

        order.OrderItems.Add(
            CreateOrderItem(
                productNameAr:
                    "منتج محفوظ ثان" ,

                productNameEn:
                    "Saved Product Two" ,

                sku:
                    "TEST-102" ,

                variantSummaryAr:
                    string.Empty ,

                variantSummaryEn:
                    string.Empty ,

                unitPrice:
                    50m ,

                discountAmount:
                    0m ,

                quantity:
                    3 ,

                lineTotal:
                    150m));

        order.OrderItems.Add(
            CreateOrderItem(
                productNameAr:
                    "منتج محذوف" ,

                productNameEn:
                    "Deleted Product" ,

                sku:
                    "TEST-DELETED" ,

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