using Mawasem.Application.Features.Orders.Contracts.Requests;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Orders;
using Mawasem.Tests.Checkout;

namespace Mawasem.Tests.Orders;

public sealed class OrderQueryServiceTests
{
    private static readonly DateTime BaseOrderDateUtc =
        new(
            2026 ,
            7 ,
            20 ,
            12 ,
            0 ,
            0 ,
            DateTimeKind.Utc);

    [Fact]
    public async Task GetCustomerListAsync_ValidRequest_ReturnsOnlyOwnedOrdersNewestFirst()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        await SeedOrdersAsync(database);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest
                {
                    PageNumber = 1 ,
                    PageSize = 20
                });

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        var response =
            result.Response;

        Assert.Equal(
            2 ,
            response.TotalCount);

        Assert.Equal(
            1 ,
            response.TotalPages);

        Assert.Equal(
            2 ,
            response.Items.Count);

        var items =
            response.Items.ToArray();

        Assert.Equal(
            "MWS-CUSTOMER-NEW" ,
            items[0].OrderNumber);

        Assert.Equal(
            "MWS-CUSTOMER-OLD" ,
            items[1].OrderNumber);

        Assert.DoesNotContain(
            response.Items ,
            item =>
                item.OrderNumber ==
                "MWS-OTHER-CUSTOMER");

        Assert.DoesNotContain(
            response.Items ,
            item =>
                item.OrderNumber ==
                "MWS-DELETED-ORDER");
    }

    [Fact]
    public async Task GetCustomerListAsync_OrderWithDeletedItem_ExcludesDeletedItemFromCounts()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        await SeedOrdersAsync(database);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest
                {
                    Search =
                        "MWS-CUSTOMER-NEW" ,

                    PageNumber = 1 ,
                    PageSize = 20
                });

        Assert.True(result.Succeeded);

        var item =
            Assert.Single(
                result.Response!.Items);

        Assert.Equal(
            2 ,
            item.DistinctItemCount);

        Assert.Equal(
            5 ,
            item.TotalQuantity);
    }

    [Fact]
    public async Task GetCustomerListAsync_StatusAndDateFilters_ReturnMatchingOrder()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        await SeedOrdersAsync(database);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest
                {
                    Status =
                        OrderStatus.Confirmed ,

                    FromDateUtc =
                        BaseOrderDateUtc.AddDays(1) ,

                    ToDateUtc =
                        BaseOrderDateUtc.AddDays(3) ,

                    PageNumber = 1 ,
                    PageSize = 20
                });

        Assert.True(result.Succeeded);

        var item =
            Assert.Single(
                result.Response!.Items);

        Assert.Equal(
            "MWS-CUSTOMER-NEW" ,
            item.OrderNumber);

        Assert.Equal(
            OrderStatus.Confirmed ,
            item.OrderStatus);

        Assert.False(
            item.CanCancel);
    }

    [Fact]
    public async Task GetCustomerListAsync_PendingOrder_CanBeCancelled()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        await SeedOrdersAsync(database);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest
                {
                    Status =
                        OrderStatus.Pending ,

                    PageNumber = 1 ,
                    PageSize = 20
                });

        Assert.True(result.Succeeded);

        var item =
            Assert.Single(
                result.Response!.Items);

        Assert.Equal(
            "MWS-CUSTOMER-OLD" ,
            item.OrderNumber);

        Assert.True(
            item.CanCancel);
    }

    [Fact]
    public async Task GetCustomerListAsync_Pagination_ReturnsRequestedPage()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart = false
            });

        await SeedOrdersAsync(database);

        await using var dbContext =
            database.CreateContext();

        var service =
            new OrderQueryService(dbContext);

        var result =
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest
                {
                    PageNumber = 2 ,
                    PageSize = 1
                });

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            2 ,
            result.Response.TotalCount);

        Assert.Equal(
            2 ,
            result.Response.TotalPages);

        Assert.Equal(
            2 ,
            result.Response.PageNumber);

        var item =
            Assert.Single(
                result.Response.Items);

        Assert.Equal(
            "MWS-CUSTOMER-OLD" ,
            item.OrderNumber);
    }

    [Fact]
    public async Task GetCustomerListAsync_BlockedCustomer_ReturnsFailure()
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
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.CustomerBlocked ,
            result.ErrorCode);

        Assert.Null(result.Response);
    }

    [Fact]
    public async Task GetCustomerListAsync_MissingCustomer_ReturnsFailure()
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
            await service.GetCustomerListAsync(
                999999 ,
                new GetCustomerOrdersRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.CustomerNotFound ,
            result.ErrorCode);
    }

    [Theory]
    [InlineData(0 , 20)]
    [InlineData(1 , 0)]
    [InlineData(1 , 101)]
    public async Task GetCustomerListAsync_InvalidPagination_ReturnsFailure(
        int pageNumber ,
        int pageSize )
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
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest
                {
                    PageNumber = pageNumber ,
                    PageSize = pageSize
                });

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.InvalidRequest ,
            result.ErrorCode);
    }

    [Fact]
    public async Task GetCustomerListAsync_InvalidDateRange_ReturnsFailure()
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
            await service.GetCustomerListAsync(
                CheckoutTestDatabase.CustomerId ,
                new GetCustomerOrdersRequest
                {
                    FromDateUtc =
                        BaseOrderDateUtc.AddDays(5) ,

                    ToDateUtc =
                        BaseOrderDateUtc
                });

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.InvalidRequest ,
            result.ErrorCode);
    }

    private static async Task SeedOrdersAsync(
        CheckoutTestDatabase database )
    {
        await using var dbContext =
            database.CreateContext();

        var olderCustomerOrder =
            CreateOrder(
                userId:
                    CheckoutTestDatabase.CustomerId ,

                orderNumber:
                    "MWS-CUSTOMER-OLD" ,

                orderDate:
                    BaseOrderDateUtc ,

                orderStatus:
                    OrderStatus.Pending ,

                subTotal:
                    100m ,

                deliveryFee:
                    25m);

        olderCustomerOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 1 ,
                unitPrice: 100m));

        var newerCustomerOrder =
            CreateOrder(
                userId:
                    CheckoutTestDatabase.CustomerId ,

                orderNumber:
                    "MWS-CUSTOMER-NEW" ,

                orderDate:
                    BaseOrderDateUtc.AddDays(2) ,

                orderStatus:
                    OrderStatus.Confirmed ,

                subTotal:
                    500m ,

                deliveryFee:
                    25m);

        newerCustomerOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 2 ,
                unitPrice: 100m));

        newerCustomerOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 3 ,
                unitPrice: 100m));

        newerCustomerOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 10 ,
                unitPrice: 100m ,
                isDeleted: true));

        var otherCustomerOrder =
            CreateOrder(
                userId:
                    CheckoutTestDatabase.OtherCustomerId ,

                orderNumber:
                    "MWS-OTHER-CUSTOMER" ,

                orderDate:
                    BaseOrderDateUtc.AddDays(3) ,

                orderStatus:
                    OrderStatus.Pending ,

                subTotal:
                    100m ,

                deliveryFee:
                    25m);

        otherCustomerOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 1 ,
                unitPrice: 100m));

        var deletedCustomerOrder =
            CreateOrder(
                userId:
                    CheckoutTestDatabase.CustomerId ,

                orderNumber:
                    "MWS-DELETED-ORDER" ,

                orderDate:
                    BaseOrderDateUtc.AddDays(4) ,

                orderStatus:
                    OrderStatus.Pending ,

                subTotal:
                    100m ,

                deliveryFee:
                    25m);

        deletedCustomerOrder.IsDeleted = true;

        deletedCustomerOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 1 ,
                unitPrice: 100m));

        dbContext.Orders.AddRange(
            olderCustomerOrder ,
            newerCustomerOrder ,
            otherCustomerOrder ,
            deletedCustomerOrder);

        await dbContext.SaveChangesAsync();
    }

    private static Order CreateOrder(
        int userId ,
        string orderNumber ,
        DateTime orderDate ,
        OrderStatus orderStatus ,
        decimal subTotal ,
        decimal deliveryFee )
    {
        return new Order
        {
            UserId = userId ,

            CustomerNameAr =
                "عميل اختبار" ,

            CustomerNameEn =
                "Test Customer" ,

            CustomerPhone =
                "01000000001" ,

            UserAddressId =
                CheckoutTestDatabase.AddressId ,

            ShippingDeliveryAreaId =
                CheckoutTestDatabase.DeliveryAreaId ,

            ShippingRecipientName =
                "Test Recipient" ,

            ShippingRecipientPhone =
                "01000000001" ,

            ShippingCity =
                "Cairo" ,

            ShippingAreaName =
                "Test Area" ,

            ShippingDetailedAddress =
                "10 Test Street" ,

            ShippingDeliveryAreaNameAr =
                "منطقة اختبار" ,

            ShippingDeliveryAreaNameEn =
                "Test Area" ,

            OrderNumber =
                orderNumber ,

            OrderDate =
                orderDate ,

            SubTotal =
                subTotal ,

            Discount =
                0m ,

            DeliveryFee =
                deliveryFee ,

            TotalAmount =
                subTotal + deliveryFee ,

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

            CreatedOn =
                new DateTimeOffset(
                    orderDate) ,

            CreatedBy =
                "test"
        };
    }

    private static OrderItem CreateOrderItem(
        int quantity ,
        decimal unitPrice ,
        bool isDeleted = false )
    {
        return new OrderItem
        {
            ProductId =
                CheckoutTestDatabase.ProductId ,

            ProductVariantId =
                CheckoutTestDatabase.ProductVariantId ,

            ProductNameAr =
                "منتج اختبار" ,

            ProductNameEn =
                "Test Product" ,

            SKU =
                "TEST-101" ,

            VariantSummaryAr =
                string.Empty ,

            VariantSummaryEn =
                string.Empty ,

            UnitPrice =
                unitPrice ,

            DiscountAmount =
                0m ,

            Quantity =
                quantity ,

            TotalPrice =
                unitPrice * quantity ,

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