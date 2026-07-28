using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;
using Mawasem.Infrastructure.StoreOrders;
using Mawasem.Tests.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.StoreOrders;

public sealed class StorePickupCollectionServiceTests
{
    [Fact]
    public async Task CollectAsync_CashPayment_MarksOrderPaidAndDelivered()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePickupOrderAsync(
                database ,
                "pickup-cash-collection");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateCollectionService(dbContext)
                    .CollectAsync(
                        orderId ,
                        CheckoutTestDatabase.DashboardUserId ,
                        new CollectStorePickupOrderRequest
                        {
                            PaymentMethod =
                                PaymentMethod.CashAtStore ,

                            Notes =
                                "Customer collected the order."
                        });

            Assert.True(
                result.Succeeded ,
                $"{result.ErrorCode}: {result.ErrorMessage}");

            Assert.NotNull(
                result.Response);

            Assert.Equal(
                OrderStatus.Delivered ,
                result.Response.OrderStatus);

            Assert.Equal(
                PaymentStatus.Paid ,
                result.Response.PaymentStatus);

            Assert.Equal(
                PaymentMethod.CashAtStore ,
                result.Response.PaymentMethod);

            Assert.Null(
                result.Response.PaymentReference);

            Assert.NotNull(
                result.Response.PaidAtUtc);

            Assert.Equal(
                CheckoutTestDatabase.DashboardUserId ,
                result.Response.CollectedByEmployeeId);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .Include(candidate =>
                    candidate.StatusHistory)
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            OrderStatus.Delivered ,
            order.OrderStatus);

        Assert.Equal(
            PaymentStatus.Paid ,
            order.PaymentStatus);

        Assert.Equal(
            PaymentMethod.CashAtStore ,
            order.PaymentMethod);

        Assert.NotNull(
            order.PaidAtUtc);

        Assert.Equal(
            "DashboardUser:50" ,
            order.LastModifiedBy);

        var history =
            Assert.Single(
                order.StatusHistory);

        Assert.Equal(
            OrderStatus.Pending ,
            history.PreviousStatus);

        Assert.Equal(
            OrderStatus.Delivered ,
            history.NewStatus);

        Assert.Equal(
            CheckoutTestDatabase.DashboardUserId ,
            history.ChangedByUserId);

        Assert.Equal(
            OrderStatusChangeActorType.DashboardUser ,
            history.ActorType);
    }

    [Fact]
    public async Task CollectAsync_CardPayment_RecordsPaymentReference()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePickupOrderAsync(
                database ,
                "pickup-card-collection");

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateCollectionService(dbContext)
                .CollectAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId ,
                    new CollectStorePickupOrderRequest
                    {
                        PaymentMethod =
                            PaymentMethod.CardAtStore ,

                        PaymentReference =
                            "  TERMINAL-12345  "
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        Assert.Equal(
            PaymentMethod.CardAtStore ,
            result.Response.PaymentMethod);

        Assert.Equal(
            "TERMINAL-12345" ,
            result.Response.PaymentReference);

        Assert.Equal(
            PaymentStatus.Paid ,
            result.Response.PaymentStatus);
    }

    [Fact]
    public async Task CollectAsync_CardWithoutReference_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePickupOrderAsync(
                database ,
                "pickup-missing-card-reference");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateCollectionService(dbContext)
                    .CollectAsync(
                        orderId ,
                        CheckoutTestDatabase.DashboardUserId ,
                        new CollectStorePickupOrderRequest
                        {
                            PaymentMethod =
                                PaymentMethod.CardAtStore ,

                            PaymentReference =
                                null
                        });

            Assert.False(
                result.Succeeded);

            Assert.Equal(
                StorePickupCollectionErrorCodes
                    .PaymentReferenceRequired ,
                result.ErrorCode);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            OrderStatus.Pending ,
            order.OrderStatus);

        Assert.Equal(
            PaymentStatus.Pending ,
            order.PaymentStatus);

        Assert.Null(
            order.PaidAtUtc);
    }

    [Fact]
    public async Task CollectAsync_DoesNotDeductStockAgain()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                StockQuantity =
                    10 ,

                CartQuantity =
                    3
            });

        var orderId =
            await CreatePickupOrderAsync(
                database ,
                "pickup-no-double-stock-deduction");

        await using (
            var collectionContext =
                database.CreateContext() )
        {
            var result =
                await CreateCollectionService(
                        collectionContext)
                    .CollectAsync(
                        orderId ,
                        CheckoutTestDatabase.DashboardUserId ,
                        new CollectStorePickupOrderRequest
                        {
                            PaymentMethod =
                                PaymentMethod.InstaPayAtStore ,

                            PaymentReference =
                                "INSTAPAY-98765"
                        });

            Assert.True(
                result.Succeeded ,
                $"{result.ErrorCode}: {result.ErrorMessage}");
        }

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync(candidate =>
                    candidate.Id ==
                    CheckoutTestDatabase.ProductVariantId);

        Assert.Equal(
            7 ,
            variant.StockQuantity);
    }

    [Fact]
    public async Task CollectAsync_HomeDeliveryOrder_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreateHomeDeliveryOrderAsync(
                database ,
                "home-delivery-not-pickup");

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateCollectionService(dbContext)
                .CollectAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId ,
                    new CollectStorePickupOrderRequest
                    {
                        PaymentMethod =
                            PaymentMethod.CashAtStore
                    });

        Assert.False(
            result.Succeeded);

        Assert.Equal(
            StorePickupCollectionErrorCodes
                .NotStorePickupOrder ,
            result.ErrorCode);
    }

    [Fact]
    public async Task CollectAsync_SamePaymentReplay_ReturnsExistingResult()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        var orderId =
            await CreatePickupOrderAsync(
                database ,
                "pickup-idempotent-collection");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateCollectionService(dbContext);

            var request =
                new CollectStorePickupOrderRequest
                {
                    PaymentMethod =
                        PaymentMethod.InstaPayAtStore ,

                    PaymentReference =
                        "INSTAPAY-REPLAY"
                };

            var first =
                await service.CollectAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId ,
                    request);

            var second =
                await service.CollectAsync(
                    orderId ,
                    CheckoutTestDatabase.DashboardUserId ,
                    request);

            Assert.True(
                first.Succeeded ,
                $"{first.ErrorCode}: {first.ErrorMessage}");

            Assert.True(
                second.Succeeded ,
                $"{second.ErrorCode}: {second.ErrorMessage}");

            Assert.Equal(
                first.Response!.OrderId ,
                second.Response!.OrderId);

            Assert.Equal(
                first.Response.PaidAtUtc ,
                second.Response.PaidAtUtc);
        }

        await using var verificationContext =
            database.CreateContext();

        var historyCount =
            await verificationContext
                .OrderStatusHistories
                .CountAsync(history =>
                    history.OrderId == orderId &&
                    history.NewStatus ==
                        OrderStatus.Delivered);

        Assert.Equal(
            1 ,
            historyCount);
    }

    private static async Task<int> CreatePickupOrderAsync(
        CheckoutTestDatabase database ,
        string idempotencyKey )
    {
        await using var dbContext =
            database.CreateContext();

        var result =
            await new CheckoutService(
                    dbContext ,
                    TimeProvider.System)
                .PlaceOrderAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new PlaceOrderRequest
                    {
                        UserAddressId =
                            null ,

                        DeliveryMethod =
                            DeliveryMethod.StorePickup ,

                        PaymentMethod =
                            PaymentMethod.CashAtStore ,

                        IdempotencyKey =
                            idempotencyKey
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        return result.Response.OrderId;
    }

    private static async Task<int>
        CreateHomeDeliveryOrderAsync(
            CheckoutTestDatabase database ,
            string idempotencyKey )
    {
        await using var dbContext =
            database.CreateContext();

        var result =
            await new CheckoutService(
                    dbContext ,
                    TimeProvider.System)
                .PlaceOrderAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new PlaceOrderRequest
                    {
                        UserAddressId =
                            CheckoutTestDatabase.AddressId ,

                        DeliveryMethod =
                            DeliveryMethod.HomeDelivery ,

                        PaymentMethod =
                            PaymentMethod.CashOnDelivery ,

                        IdempotencyKey =
                            idempotencyKey
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        return result.Response.OrderId;
    }

    private static StorePickupCollectionService
        CreateCollectionService(
            CheckoutTestDbContext dbContext )
    {
        return new StorePickupCollectionService(
            dbContext ,
            TimeProvider.System);
    }
}