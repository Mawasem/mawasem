using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;
using Mawasem.Infrastructure.Orders;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.Checkout;

public sealed class OrderStatusHistoryTests
{
    [Fact]
    public async Task
        ConfirmAsync_PendingOrder_RecordsDashboardEmployeeHistory()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "history-confirm-order");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateWorkflowService(dbContext)
                    .ConfirmAsync(
                        orderId ,
                        CheckoutTestDatabase.DashboardUserId);

            Assert.True(result.Succeeded);
            Assert.True(result.Response!.StatusChanged);
        }

        await using var verificationContext =
            database.CreateContext();

        var history =
            await verificationContext
                .OrderStatusHistories
                .SingleAsync(candidate =>
                    candidate.OrderId == orderId);

        Assert.Equal(
            OrderStatus.Pending ,
            history.PreviousStatus);

        Assert.Equal(
            OrderStatus.Confirmed ,
            history.NewStatus);

        Assert.Equal(
            CheckoutTestDatabase.DashboardUserId ,
            history.ChangedByUserId);

        Assert.Equal(
            OrderStatusChangeActorType.DashboardUser ,
            history.ActorType);

        Assert.Null(history.Reason);
    }

    [Fact]
    public async Task
        DeliverAsync_CompletedWorkflow_RecordsEveryEmployeeAction()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "history-delivered-order");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(dbContext);

            Assert.True(
                (await service.ConfirmAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId))
                .Succeeded);

            Assert.True(
                (await service.PrepareAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId))
                .Succeeded);

            Assert.True(
                (await service.ShipAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId))
                .Succeeded);

            Assert.True(
                (await service.DeliverAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId))
                .Succeeded);
        }

        await using var verificationContext =
            database.CreateContext();

        var history =
            await verificationContext
                .OrderStatusHistories
                .Where(candidate =>
                    candidate.OrderId == orderId)
                .OrderBy(candidate =>
                    candidate.Id)
                .ToArrayAsync();

        Assert.Equal(
            4 ,
            history.Length);

        Assert.Collection(
            history ,
            entry =>
            {
                Assert.Equal(
                    OrderStatus.Pending ,
                    entry.PreviousStatus);

                Assert.Equal(
                    OrderStatus.Confirmed ,
                    entry.NewStatus);
            } ,
            entry =>
            {
                Assert.Equal(
                    OrderStatus.Confirmed ,
                    entry.PreviousStatus);

                Assert.Equal(
                    OrderStatus.Preparing ,
                    entry.NewStatus);
            } ,
            entry =>
            {
                Assert.Equal(
                    OrderStatus.Preparing ,
                    entry.PreviousStatus);

                Assert.Equal(
                    OrderStatus.Shipped ,
                    entry.NewStatus);
            } ,
            entry =>
            {
                Assert.Equal(
                    OrderStatus.Shipped ,
                    entry.PreviousStatus);

                Assert.Equal(
                    OrderStatus.Delivered ,
                    entry.NewStatus);
            });

        Assert.All(
            history ,
            entry =>
            {
                Assert.Equal(
                    CheckoutTestDatabase.DashboardUserId ,
                    entry.ChangedByUserId);

                Assert.Equal(
                    OrderStatusChangeActorType.DashboardUser ,
                    entry.ActorType);
            });
    }

    [Fact]
    public async Task
        CancelByDashboardAsync_RepeatedRequest_RecordsOneEmployeeAction()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "history-dashboard-cancel");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateWorkflowService(dbContext);

            var firstResult =
                await service.CancelByDashboardAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId ,
                    "Customer was unavailable");

            var repeatedResult =
                await service.CancelByDashboardAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId ,
                    "Repeated cancellation");

            Assert.True(firstResult.Succeeded);
            Assert.True(firstResult.Response!.StatusChanged);

            Assert.True(repeatedResult.Succeeded);
            Assert.False(repeatedResult.Response!.StatusChanged);
        }

        await using var verificationContext =
            database.CreateContext();

        var history =
            await verificationContext
                .OrderStatusHistories
                .SingleAsync(candidate =>
                    candidate.OrderId == orderId);

        Assert.Equal(
            OrderStatus.Cancelled ,
            history.NewStatus);

        Assert.Equal(
            CheckoutTestDatabase.DashboardUserId ,
            history.ChangedByUserId);

        Assert.Equal(
            OrderStatusChangeActorType.DashboardUser ,
            history.ActorType);

        Assert.Equal(
            "Customer was unavailable" ,
            history.Reason);
    }

    [Fact]
    public async Task
        CancelByCustomerAsync_OwnOrder_RecordsCustomerAction()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePendingOrderAsync(
                database ,
                "history-customer-cancel");

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
            Assert.True(result.Response!.StatusChanged);
        }

        await using var verificationContext =
            database.CreateContext();

        var history =
            await verificationContext
                .OrderStatusHistories
                .SingleAsync(candidate =>
                    candidate.OrderId == orderId);

        Assert.Equal(
            CheckoutTestDatabase.CustomerId ,
            history.ChangedByUserId);

        Assert.Equal(
            OrderStatusChangeActorType.Customer ,
            history.ActorType);

        Assert.Equal(
            OrderStatus.Cancelled ,
            history.NewStatus);

        Assert.Equal(
            "Changed my mind" ,
            history.Reason);
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
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(result.Response);

        return result.Response!.OrderId;
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
