using Mawasem.Application.Features.Orders.Contracts.Requests;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Orders;
using Mawasem.Tests.Checkout;

namespace Mawasem.Tests.Orders;

public sealed class AdminOrderListQueryTests
{
    private static readonly DateTime BaseOrderDateUtc =
        new(
            2026 ,
            7 ,
            25 ,
            12 ,
            0 ,
            0 ,
            DateTimeKind.Utc);

    [Fact]
    public async Task GetAdminListAsync_ValidRequest_ReturnsAllActiveOrdersNewestFirst()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    PageNumber = 1 ,
                    PageSize = 20
                });

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            3 ,
            result.Response.TotalCount);

        Assert.Equal(
            1 ,
            result.Response.TotalPages);

        var items =
            result.Response.Items.ToArray();

        Assert.Equal(
            3 ,
            items.Length);

        Assert.Equal(
            "MWS-ADMIN-CANCELLED" ,
            items[0].OrderNumber);

        Assert.Equal(
            "MWS-ADMIN-CONFIRMED" ,
            items[1].OrderNumber);

        Assert.Equal(
            "MWS-ADMIN-PENDING" ,
            items[2].OrderNumber);

        Assert.DoesNotContain(
            items ,
            item =>
                item.OrderNumber ==
                "MWS-ADMIN-DELETED");
    }

    [Fact]
    public async Task GetAdminListAsync_ReturnsCorrectActionAvailability()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        var pending =
            Assert.Single(
                result.Response.Items ,
                item =>
                    item.OrderStatus ==
                    OrderStatus.Pending);

        Assert.True(pending.CanConfirm);
        Assert.True(pending.CanReject);
        Assert.True(pending.CanCancel);

        var confirmed =
            Assert.Single(
                result.Response.Items ,
                item =>
                    item.OrderStatus ==
                    OrderStatus.Confirmed);

        Assert.False(confirmed.CanConfirm);
        Assert.False(confirmed.CanReject);
        Assert.True(confirmed.CanCancel);

        var cancelled =
            Assert.Single(
                result.Response.Items ,
                item =>
                    item.OrderStatus ==
                    OrderStatus.Cancelled);

        Assert.False(cancelled.CanConfirm);
        Assert.False(cancelled.CanReject);
        Assert.False(cancelled.CanCancel);
    }

    [Fact]
    public async Task GetAdminListAsync_DeletedOrderItem_ExcludesItFromCounts()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    Search =
                        "MWS-ADMIN-PENDING"
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

    [Theory]
    [InlineData("MWS-ADMIN-PENDING")]
    [InlineData("Customer Pending")]
    [InlineData("عميل الطلب المعلق")]
    [InlineData("+201000000011")]
    public async Task GetAdminListAsync_Search_ReturnsMatchingOrder(
        string search )
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    Search = search
                });

        Assert.True(result.Succeeded);

        var item =
            Assert.Single(
                result.Response!.Items);

        Assert.Equal(
            "MWS-ADMIN-PENDING" ,
            item.OrderNumber);
    }

    [Fact]
    public async Task GetAdminListAsync_AllFilters_ReturnMatchingOrder()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    CustomerUserId =
                        CheckoutTestDatabase.CustomerId ,

                    Status =
                        OrderStatus.Confirmed ,

                    PaymentMethod =
                        PaymentMethod.CashOnDelivery ,

                    PaymentStatus =
                        PaymentStatus.Pending ,

                    DeliveryMethod =
                        DeliveryMethod.HomeDelivery ,

                    OrderSource =
                        OrderSource.Website ,

                    DeliveryAreaId =
                        CheckoutTestDatabase.DeliveryAreaId ,

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
            "MWS-ADMIN-CONFIRMED" ,
            item.OrderNumber);

        Assert.Equal(
            CheckoutTestDatabase.CustomerId ,
            item.CustomerUserId);

        Assert.Equal(
            "Customer Confirmed" ,
            item.CustomerNameEn);

        Assert.Equal(
            "عميل الطلب المؤكد" ,
            item.CustomerNameAr);

        Assert.Equal(
            "+201000000012" ,
            item.CustomerPhone);
    }

    [Fact]
    public async Task GetAdminListAsync_Pagination_ReturnsRequestedPage()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    PageNumber = 2 ,
                    PageSize = 1
                });

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            3 ,
            result.Response.TotalCount);

        Assert.Equal(
            3 ,
            result.Response.TotalPages);

        Assert.Equal(
            2 ,
            result.Response.PageNumber);

        var item =
            Assert.Single(
                result.Response.Items);

        Assert.Equal(
            "MWS-ADMIN-CONFIRMED" ,
            item.OrderNumber);
    }

    [Theory]
    [InlineData(0 , 20)]
    [InlineData(1 , 0)]
    [InlineData(1 , 101)]
    public async Task GetAdminListAsync_InvalidPagination_ReturnsFailure(
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
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
    public async Task GetAdminListAsync_InvalidCustomerId_ReturnsFailure()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    CustomerUserId = 0
                });

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.InvalidRequest ,
            result.ErrorCode);
    }

    [Fact]
    public async Task GetAdminListAsync_InvalidDeliveryAreaId_ReturnsFailure()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    DeliveryAreaId = -1
                });

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.InvalidRequest ,
            result.ErrorCode);
    }

    [Fact]
    public async Task GetAdminListAsync_InvalidEnum_ReturnsFailure()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
                {
                    Status =
                        (OrderStatus)999
                });

        Assert.False(result.Succeeded);

        Assert.Equal(
            OrderQueryErrorCodes.InvalidRequest ,
            result.ErrorCode);
    }

    [Fact]
    public async Task GetAdminListAsync_InvalidDateRange_ReturnsFailure()
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
            await service.GetAdminListAsync(
                new GetAdminOrdersRequest
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

        var pendingOrder =
            CreateOrder(
                customerUserId:
                    CheckoutTestDatabase.CustomerId ,

                orderNumber:
                    "MWS-ADMIN-PENDING" ,

                orderDate:
                    BaseOrderDateUtc ,

                orderStatus:
                    OrderStatus.Pending ,

                customerNameAr:
                    "عميل الطلب المعلق" ,

                customerNameEn:
                    "Customer Pending" ,

                customerPhone:
                    "+201000000011");

        pendingOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 2 ,
                unitPrice: 100m));

        pendingOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 3 ,
                unitPrice: 50m));

        pendingOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 20 ,
                unitPrice: 10m ,
                isDeleted: true));

        var confirmedOrder =
            CreateOrder(
                customerUserId:
                    CheckoutTestDatabase.CustomerId ,

                orderNumber:
                    "MWS-ADMIN-CONFIRMED" ,

                orderDate:
                    BaseOrderDateUtc.AddDays(2) ,

                orderStatus:
                    OrderStatus.Confirmed ,

                customerNameAr:
                    "عميل الطلب المؤكد" ,

                customerNameEn:
                    "Customer Confirmed" ,

                customerPhone:
                    "+201000000012");

        confirmedOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 1 ,
                unitPrice: 200m));

        var cancelledOrder =
            CreateOrder(
                customerUserId:
                    CheckoutTestDatabase.OtherCustomerId ,

                orderNumber:
                    "MWS-ADMIN-CANCELLED" ,

                orderDate:
                    BaseOrderDateUtc.AddDays(4) ,

                orderStatus:
                    OrderStatus.Cancelled ,

                customerNameAr:
                    "عميل الطلب الملغي" ,

                customerNameEn:
                    "Customer Cancelled" ,

                customerPhone:
                    "+201000000013");

        cancelledOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 4 ,
                unitPrice: 75m));

        var deletedOrder =
            CreateOrder(
                customerUserId:
                    CheckoutTestDatabase.CustomerId ,

                orderNumber:
                    "MWS-ADMIN-DELETED" ,

                orderDate:
                    BaseOrderDateUtc.AddDays(6) ,

                orderStatus:
                    OrderStatus.Pending ,

                customerNameAr:
                    "عميل الطلب المحذوف" ,

                customerNameEn:
                    "Customer Deleted" ,

                customerPhone:
                    "+201000000014");

        deletedOrder.IsDeleted = true;

        deletedOrder.OrderItems.Add(
            CreateOrderItem(
                quantity: 1 ,
                unitPrice: 100m));

        dbContext.Orders.AddRange(
            pendingOrder ,
            confirmedOrder ,
            cancelledOrder ,
            deletedOrder);

        await dbContext.SaveChangesAsync();
    }

    private static Order CreateOrder(
        int customerUserId ,
        string orderNumber ,
        DateTime orderDate ,
        OrderStatus orderStatus ,
        string customerNameAr ,
        string customerNameEn ,
        string customerPhone )
    {
        return new Order
        {
            UserId =
                customerUserId ,

            CustomerNameAr =
                customerNameAr ,

            CustomerNameEn =
                customerNameEn ,

            CustomerPhone =
                customerPhone ,

            UserAddressId =
                CheckoutTestDatabase.AddressId ,

            ShippingDeliveryAreaId =
                CheckoutTestDatabase.DeliveryAreaId ,

            ShippingRecipientName =
                customerNameEn ,

            ShippingRecipientPhone =
                customerPhone ,

            ShippingCity =
                "Cairo" ,

            ShippingAreaName =
                "Nasr City" ,

            ShippingDetailedAddress =
                "10 Test Street" ,

            ShippingDeliveryAreaNameAr =
                "القاهرة" ,

            ShippingDeliveryAreaNameEn =
                "Cairo" ,

            OrderNumber =
                orderNumber ,

            OrderDate =
                orderDate ,

            SubTotal =
                300m ,

            Discount =
                0m ,

            DeliveryFee =
                50m ,

            TotalAmount =
                350m ,

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
                new DateTimeOffset(orderDate) ,

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
                "TEST-ADMIN-ORDER" ,

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