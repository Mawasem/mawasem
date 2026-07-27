using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;
using Mawasem.Infrastructure.Orders;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.Checkout;

public sealed class OrderStatusProgressionTests
{
    private const int DashboardUserId = 50;

    [Theory]
    [InlineData(
        ProgressionAction.Prepare ,
        OrderStatus.Confirmed ,
        OrderStatus.Preparing)]
    [InlineData(
        ProgressionAction.Ship ,
        OrderStatus.Preparing ,
        OrderStatus.Shipped)]
    [InlineData(
        ProgressionAction.Deliver ,
        OrderStatus.Shipped ,
        OrderStatus.Delivered)]
    public async Task ProgressionAsync_ExpectedStatus_TransitionsForward(
        ProgressionAction action ,
        OrderStatus initialStatus ,
        OrderStatus targetStatus )
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreateOrderWithStatusAsync(
                database ,
                $"valid-{action}" ,
                initialStatus);

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(
                    dbContext);

            var result =
                await ExecuteAsync(
                    service ,
                    action ,
                    orderId);

            Assert.True(
                result.Succeeded);

            Assert.NotNull(
                result.Response);

            Assert.Equal(
                initialStatus ,
                result.Response!.PreviousStatus);

            Assert.Equal(
                targetStatus ,
                result.Response.CurrentStatus);

            Assert.True(
                result.Response.StatusChanged);

            Assert.False(
                result.Response.StockRestored);

            Assert.Null(
                result.Response.StockRestoredAtUtc);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            targetStatus ,
            order.OrderStatus);

        Assert.Null(
            order.StockRestoredAtUtc);

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);
    }

    [Theory]
    [InlineData(
        ProgressionAction.Prepare ,
        OrderStatus.Confirmed ,
        OrderStatus.Preparing)]
    [InlineData(
        ProgressionAction.Ship ,
        OrderStatus.Preparing ,
        OrderStatus.Shipped)]
    [InlineData(
        ProgressionAction.Deliver ,
        OrderStatus.Shipped ,
        OrderStatus.Delivered)]
    public async Task ProgressionAsync_RepeatedRequest_IsIdempotent(
        ProgressionAction action ,
        OrderStatus initialStatus ,
        OrderStatus targetStatus )
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreateOrderWithStatusAsync(
                database ,
                $"repeated-{action}" ,
                initialStatus);

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateWorkflowService(
                dbContext);

        var firstResult =
            await ExecuteAsync(
                service ,
                action ,
                orderId);

        var secondResult =
            await ExecuteAsync(
                service ,
                action ,
                orderId);

        Assert.True(
            firstResult.Succeeded);

        Assert.True(
            secondResult.Succeeded);

        Assert.NotNull(
            firstResult.Response);

        Assert.NotNull(
            secondResult.Response);

        Assert.Equal(
            initialStatus ,
            firstResult.Response!.PreviousStatus);

        Assert.Equal(
            targetStatus ,
            firstResult.Response.CurrentStatus);

        Assert.True(
            firstResult.Response.StatusChanged);

        Assert.False(
            firstResult.Response.StockRestored);

        Assert.Equal(
            targetStatus ,
            secondResult.Response!.PreviousStatus);

        Assert.Equal(
            targetStatus ,
            secondResult.Response.CurrentStatus);

        Assert.False(
            secondResult.Response.StatusChanged);

        Assert.False(
            secondResult.Response.StockRestored);

        Assert.Null(
            secondResult.Response.StockRestoredAtUtc);

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            targetStatus ,
            order.OrderStatus);

        Assert.Null(
            order.StockRestoredAtUtc);

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);
    }

    [Theory]
    [InlineData(
        ProgressionAction.Prepare ,
        OrderStatus.Pending)]
    [InlineData(
        ProgressionAction.Prepare ,
        OrderStatus.Rejected)]
    [InlineData(
        ProgressionAction.Ship ,
        OrderStatus.Confirmed)]
    [InlineData(
        ProgressionAction.Ship ,
        OrderStatus.Cancelled)]
    [InlineData(
        ProgressionAction.Deliver ,
        OrderStatus.Preparing)]
    public async Task ProgressionAsync_InvalidStartingStatus_IsRejected(
        ProgressionAction action ,
        OrderStatus initialStatus )
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreateOrderWithStatusAsync(
                database ,
                $"invalid-{action}-{initialStatus}" ,
                initialStatus);

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(
                    dbContext);

            var result =
                await ExecuteAsync(
                    service ,
                    action ,
                    orderId);

            Assert.False(
                result.Succeeded);

            Assert.Equal(
                OrderWorkflowErrorCodes
                    .InvalidStatusTransition ,
                result.ErrorCode);

            Assert.Null(
                result.Response);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            initialStatus ,
            order.OrderStatus);

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);
    }

    [Theory]
    [InlineData(ProgressionAction.Prepare)]
    [InlineData(ProgressionAction.Ship)]
    [InlineData(ProgressionAction.Deliver)]
    public async Task ProgressionAsync_MissingOrder_ReturnsNotFound(
        ProgressionAction action )
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateWorkflowService(
                dbContext);

        var result =
            await ExecuteAsync(
                service ,
                action ,
                999999);

        Assert.False(
            result.Succeeded);

        Assert.Equal(
            OrderWorkflowErrorCodes.OrderNotFound ,
            result.ErrorCode);

        Assert.Null(
            result.Response);
    }

    [Fact]
    public async Task PrepareAsync_DeletedOrder_ReturnsNotFound()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreateOrderWithStatusAsync(
                database ,
                "deleted-order" ,
                OrderStatus.Confirmed);

        await SoftDeleteOrderAsync(
            database ,
            orderId);

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateWorkflowService(
                dbContext);

        var result =
            await service.PrepareAsync(
                orderId ,
                DashboardUserId);

        Assert.False(
            result.Succeeded);

        Assert.Equal(
            OrderWorkflowErrorCodes.OrderNotFound ,
            result.ErrorCode);

        Assert.Null(
            result.Response);

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.True(
            order.IsDeleted);

        Assert.Equal(
            OrderStatus.Confirmed ,
            order.OrderStatus);

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);
    }

    [Fact]
    public async Task ProgressionAsync_ConfirmedThroughDelivered_CompletesInOrder()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "complete-progression");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(
                    dbContext);

            var confirmation =
                await service.ConfirmAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                confirmation.Succeeded);

            Assert.Equal(
                OrderStatus.Pending ,
                confirmation.Response!.PreviousStatus);

            Assert.Equal(
                OrderStatus.Confirmed ,
                confirmation.Response.CurrentStatus);

            Assert.True(
                confirmation.Response.StatusChanged);

            var preparation =
                await service.PrepareAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                preparation.Succeeded);

            Assert.Equal(
                OrderStatus.Confirmed ,
                preparation.Response!.PreviousStatus);

            Assert.Equal(
                OrderStatus.Preparing ,
                preparation.Response.CurrentStatus);

            Assert.True(
                preparation.Response.StatusChanged);

            var shipment =
                await service.ShipAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                shipment.Succeeded);

            Assert.Equal(
                OrderStatus.Preparing ,
                shipment.Response!.PreviousStatus);

            Assert.Equal(
                OrderStatus.Shipped ,
                shipment.Response.CurrentStatus);

            Assert.True(
                shipment.Response.StatusChanged);

            var delivery =
                await service.DeliverAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                delivery.Succeeded);

            Assert.Equal(
                OrderStatus.Shipped ,
                delivery.Response!.PreviousStatus);

            Assert.Equal(
                OrderStatus.Delivered ,
                delivery.Response.CurrentStatus);

            Assert.True(
                delivery.Response.StatusChanged);

            Assert.False(
                confirmation.Response.StockRestored);

            Assert.False(
                preparation.Response.StockRestored);

            Assert.False(
                shipment.Response.StockRestored);

            Assert.False(
                delivery.Response.StockRestored);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            OrderStatus.Delivered ,
            order.OrderStatus);

        Assert.Null(
            order.StockRestoredAtUtc);

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);
    }

    [Fact]
    public async Task
        DeliverAsync_CashOnDeliveryOrder_MarksPaymentAsPaid()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreateOrderWithStatusAsync(
                database ,
                "deliver-cod-payment-status" ,
                OrderStatus.Shipped);

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(
                    dbContext);

            var result =
                await service.DeliverAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                result.Succeeded);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            OrderStatus.Delivered ,
            order.OrderStatus);

        Assert.Equal(
            PaymentMethod.CashOnDelivery ,
            order.PaymentMethod);

        Assert.Equal(
            PaymentStatus.Paid ,
            order.PaymentStatus);
    }
    private static async Task<int>
        CreateOrderWithStatusAsync(
            CheckoutTestDatabase database ,
            string idempotencyKey ,
            OrderStatus orderStatus )
    {
        var orderId =
            await CreatePendingOrderAsync(
                database ,
                idempotencyKey);

        if ( orderStatus == OrderStatus.Pending )
        {
            return orderId;
        }

        await using var dbContext =
            database.CreateContext();

        var order =
            await dbContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        order.OrderStatus =
            orderStatus;

        await dbContext.SaveChangesAsync();

        return orderId;
    }

    private static async Task<int>
        CreatePendingOrderAsync(
            CheckoutTestDatabase database ,
            string idempotencyKey )
    {
        await using var dbContext =
            database.CreateContext();

        var checkoutService =
            new CheckoutService(
                dbContext ,
                TimeProvider.System);

        var result =
            await checkoutService.PlaceOrderAsync(
                CheckoutTestDatabase.CustomerId ,
                new PlaceOrderRequest
                {
                    UserAddressId =
                        CheckoutTestDatabase.AddressId ,

                    PaymentMethod =
                        PaymentMethod.CashOnDelivery ,

                    IdempotencyKey =
                        idempotencyKey
                });

        Assert.True(
            result.Succeeded);

        Assert.NotNull(
            result.Response);

        return result.Response!.OrderId;
    }

    private static async Task SoftDeleteOrderAsync(
        CheckoutTestDatabase database ,
        int orderId )
    {
        await using var dbContext =
            database.CreateContext();

        var order =
            await dbContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        order.IsDeleted = true;

        await dbContext.SaveChangesAsync();
    }

    private static Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        ExecuteAsync(
            OrderWorkflowService service ,
            ProgressionAction action ,
            int orderId )
    {
        return action switch
        {
            ProgressionAction.Prepare =>
                service.PrepareAsync(
                    orderId ,
                    DashboardUserId),

            ProgressionAction.Ship =>
                service.ShipAsync(
                    orderId ,
                    DashboardUserId),

            ProgressionAction.Deliver =>
                service.DeliverAsync(
                    orderId ,
                    DashboardUserId),

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(action) ,
                    action ,
                    "The progression action is not supported.")
        };
    }

    private static OrderWorkflowService
        CreateWorkflowService(
            CheckoutTestDbContext dbContext )
    {
        return new OrderWorkflowService(
            dbContext ,
            TimeProvider.System);
    }

    public enum ProgressionAction
    {
        Prepare = 1,
        Ship = 2,
        Deliver = 3
    }
}