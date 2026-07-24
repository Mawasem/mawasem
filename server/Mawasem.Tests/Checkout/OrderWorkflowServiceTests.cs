using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;
using Mawasem.Infrastructure.Orders;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.Checkout;

public sealed class OrderWorkflowServiceTests
{
    private const int DashboardUserId = 50;

    [Fact]
    public async Task ConfirmAsync_PendingOrder_DoesNotDeductStockAgain()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "confirm-order");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateWorkflowService(dbContext)
                    .ConfirmAsync(
                        orderId ,
                        DashboardUserId);

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Response);

            Assert.Equal(
                OrderStatus.Pending ,
                result.Response.PreviousStatus);

            Assert.Equal(
                OrderStatus.Confirmed ,
                result.Response.CurrentStatus);

            Assert.True(
                result.Response.StatusChanged);

            Assert.False(
                result.Response.StockRestored);
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);

        var order =
            await verificationContext.Orders
                .SingleAsync();

        Assert.Equal(
            OrderStatus.Confirmed ,
            order.OrderStatus);

        Assert.Null(
            order.StockRestoredAtUtc);
    }

    [Fact]
    public async Task RejectAsync_PendingOrder_RestoresStock()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "reject-order");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateWorkflowService(dbContext)
                    .RejectAsync(
                        orderId ,
                        DashboardUserId ,
                        "Item unavailable");

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Response);

            Assert.Equal(
                OrderStatus.Rejected ,
                result.Response.CurrentStatus);

            Assert.True(
                result.Response.StatusChanged);

            Assert.True(
                result.Response.StockRestored);

            Assert.NotNull(
                result.Response.StockRestoredAtUtc);
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            10 ,
            variant.StockQuantity);

        var order =
            await verificationContext.Orders
                .SingleAsync();

        Assert.Equal(
            OrderStatus.Rejected ,
            order.OrderStatus);

        Assert.Equal(
            "Item unavailable" ,
            order.RejectionReason);

        Assert.NotNull(
            order.RejectedAtUtc);

        Assert.NotNull(
            order.StockRestoredAtUtc);
    }

    [Fact]
    public async Task RejectAsync_RepeatedRequest_DoesNotRestoreStockTwice()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "repeat-rejection");

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateWorkflowService(dbContext);

        var firstResult =
            await service.RejectAsync(
                orderId ,
                DashboardUserId ,
                "First rejection");

        var secondResult =
            await service.RejectAsync(
                orderId ,
                DashboardUserId ,
                "Repeated rejection");

        Assert.True(firstResult.Succeeded);
        Assert.True(secondResult.Succeeded);

        Assert.True(
            firstResult.Response!.StockRestored);

        Assert.False(
            secondResult.Response!.StockRestored);

        Assert.False(
            secondResult.Response.StatusChanged);

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            10 ,
            variant.StockQuantity);

        var order =
            await verificationContext.Orders
                .SingleAsync();

        Assert.Equal(
            "First rejection" ,
            order.RejectionReason);
    }

    [Fact]
    public async Task CancelByDashboardAsync_PendingOrder_RestoresStockOnce()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "dashboard-cancellation");

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateWorkflowService(dbContext);

        var firstResult =
            await service.CancelByDashboardAsync(
                orderId ,
                DashboardUserId ,
                "Customer requested cancellation");

        var secondResult =
            await service.CancelByDashboardAsync(
                orderId ,
                DashboardUserId ,
                "Repeated cancellation");

        Assert.True(firstResult.Succeeded);
        Assert.True(secondResult.Succeeded);

        Assert.True(
            firstResult.Response!.StockRestored);

        Assert.False(
            secondResult.Response!.StockRestored);

        Assert.False(
            secondResult.Response.StatusChanged);

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            10 ,
            variant.StockQuantity);

        var order =
            await verificationContext.Orders
                .SingleAsync();

        Assert.Equal(
            OrderStatus.Cancelled ,
            order.OrderStatus);

        Assert.Equal(
            "Customer requested cancellation" ,
            order.CancellationReason);

        Assert.NotNull(
            order.CancelledAtUtc);
    }

    [Fact]
    public async Task CancelByDashboardAsync_ConfirmedOrder_RestoresStock()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "confirmed-cancellation");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(dbContext);

            var confirmation =
                await service.ConfirmAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                confirmation.Succeeded);

            var cancellation =
                await service.CancelByDashboardAsync(
                    orderId ,
                    DashboardUserId ,
                    "Cancelled before shipping");

            Assert.True(
                cancellation.Succeeded);

            Assert.Equal(
                OrderStatus.Confirmed ,
                cancellation.Response!.PreviousStatus);

            Assert.Equal(
                OrderStatus.Cancelled ,
                cancellation.Response.CurrentStatus);

            Assert.True(
                cancellation.Response.StockRestored);
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            10 ,
            variant.StockQuantity);
    }

    [Fact]
    public async Task CancelByCustomerAsync_OwnPendingOrder_RestoresStock()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "customer-cancellation");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateWorkflowService(dbContext)
                    .CancelByCustomerAsync(
                        orderId ,
                        CheckoutTestDatabase.CustomerId ,
                        "Changed my mind");

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Response);

            Assert.Equal(
                OrderStatus.Cancelled ,
                result.Response.CurrentStatus);

            Assert.True(
                result.Response.StockRestored);
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            10 ,
            variant.StockQuantity);
    }

    [Fact]
    public async Task CancelByCustomerAsync_AnotherCustomersOrder_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "ownership-check");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateWorkflowService(dbContext)
                    .CancelByCustomerAsync(
                        orderId ,
                        CheckoutTestDatabase.OtherCustomerId ,
                        "Not my order");

            Assert.False(result.Succeeded);

            Assert.Equal(
                OrderWorkflowErrorCodes.OrderAccessDenied ,
                result.ErrorCode);
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);

        var order =
            await verificationContext.Orders
                .SingleAsync();

        Assert.Equal(
            OrderStatus.Pending ,
            order.OrderStatus);
    }

    [Fact]
    public async Task CancelByCustomerAsync_ConfirmedOrder_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "customer-confirmed-order");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(dbContext);

            var confirmation =
                await service.ConfirmAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                confirmation.Succeeded);

            var cancellation =
                await service.CancelByCustomerAsync(
                    orderId ,
                    CheckoutTestDatabase.CustomerId ,
                    "Customer cancellation");

            Assert.False(
                cancellation.Succeeded);

            Assert.Equal(
                OrderWorkflowErrorCodes
                    .InvalidStatusTransition ,

                cancellation.ErrorCode);
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);

        var order =
            await verificationContext.Orders
                .SingleAsync();

        Assert.Equal(
            OrderStatus.Confirmed ,
            order.OrderStatus);

        Assert.Null(
            order.StockRestoredAtUtc);
    }

    [Fact]
    public async Task RejectAsync_ConfirmedOrder_IsRejectedWithoutRestoringStock()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "confirmed-rejection");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(dbContext);

            var confirmation =
                await service.ConfirmAsync(
                    orderId ,
                    DashboardUserId);

            Assert.True(
                confirmation.Succeeded);

            var rejection =
                await service.RejectAsync(
                    orderId ,
                    DashboardUserId ,
                    "Invalid rejection");

            Assert.False(
                rejection.Succeeded);

            Assert.Equal(
                OrderWorkflowErrorCodes
                    .InvalidStatusTransition ,

                rejection.ErrorCode);
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);

        var order =
            await verificationContext.Orders
                .SingleAsync();

        Assert.Equal(
            OrderStatus.Confirmed ,
            order.OrderStatus);

        Assert.Null(
            order.StockRestoredAtUtc);
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
                new Application.Features.Checkout
                    .Contracts.Requests.PlaceOrderRequest
                {
                    UserAddressId =
                        CheckoutTestDatabase.AddressId ,

                    PaymentMethod =
                        PaymentMethod.CashOnDelivery ,

                    IdempotencyKey =
                        idempotencyKey
                });

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        return result.Response.OrderId;
    }

    private static OrderWorkflowService
        CreateWorkflowService(
            CheckoutTestDbContext dbContext )
    {
        return new OrderWorkflowService(
            dbContext ,
            TimeProvider.System);
    }
}