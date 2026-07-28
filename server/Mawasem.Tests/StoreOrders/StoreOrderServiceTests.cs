using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.StoreOrders;
using Mawasem.Tests.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.StoreOrders;

public sealed class StoreOrderServiceTests
{
    [Fact]
    public async Task CreateAsync_CashSale_CreatesAnonymousPaidDeliveredOrder()
    {
        await using var database = new CheckoutTestDatabase();
        await database.SeedAsync();

        await using var context = database.CreateContext();

        var result = await CreateService(context).CreateAsync(
            CheckoutTestDatabase.DashboardUserId ,
            CreateRequest("pos-cash"));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);
        Assert.Equal(PaymentMethod.CashAtStore , result.Response.PaymentMethod);
        Assert.Equal(PaymentStatus.Paid , result.Response.PaymentStatus);
        Assert.Equal(200m , result.Response.TotalAmount);

        var order = await context.Orders
            .Include(order => order.OrderItems)
            .Include(order => order.StatusHistory)
            .SingleAsync();

        Assert.Null(order.UserId);
        Assert.Equal(OrderSource.Store , order.OrderSource);
        Assert.Equal(DeliveryMethod.StorePickup , order.DeliveryMethod);
        Assert.Equal(OrderStatus.Delivered , order.OrderStatus);
        Assert.Single(order.OrderItems);
        Assert.Single(order.StatusHistory);
    }

    [Fact]
    public async Task CreateAsync_CardWithoutReference_Fails()
    {
        await using var database = new CheckoutTestDatabase();
        await database.SeedAsync();

        await using var context = database.CreateContext();

        var request = CreateRequest("missing-card-reference") with
        {
            PaymentMethod = PaymentMethod.CardAtStore ,
            PaymentReference = null
        };

        var result = await CreateService(context).CreateAsync(
            CheckoutTestDatabase.DashboardUserId ,
            request);

        Assert.False(result.Succeeded);
        Assert.Equal(
            StoreOrderErrorCodes.PaymentReferenceRequired ,
            result.ErrorCode);
    }

    [Fact]
    public async Task CreateAsync_Success_DeductsSharedVariantStock()
    {
        await using var database = new CheckoutTestDatabase();

        await database.SeedAsync(new CheckoutSeedOptions
        {
            StockQuantity = 10
        });

        await using ( var context = database.CreateContext() )
        {
            var result = await CreateService(context).CreateAsync(
                CheckoutTestDatabase.DashboardUserId ,
                CreateRequest("stock-deduction") with
                {
                    Items = new[]
                    {
                        new CreateStoreOrderItemRequest
                        {
                            ProductVariantId =
                                CheckoutTestDatabase.ProductVariantId,
                            Quantity = 3
                        }
                    }
                });

            Assert.True(result.Succeeded);
        }

        await using var verificationContext = database.CreateContext();

        var variant = await verificationContext.ProductVariants
            .SingleAsync(variant =>
                variant.Id == CheckoutTestDatabase.ProductVariantId);

        Assert.Equal(7 , variant.StockQuantity);
    }

    [Fact]
    public async Task CreateAsync_InsufficientStock_FailsWithoutCreatingOrder()
    {
        await using var database = new CheckoutTestDatabase();

        await database.SeedAsync(new CheckoutSeedOptions
        {
            StockQuantity = 1
        });

        await using var context = database.CreateContext();

        var result = await CreateService(context).CreateAsync(
            CheckoutTestDatabase.DashboardUserId ,
            CreateRequest("insufficient-stock") with
            {
                Items = new[]
                {
                    new CreateStoreOrderItemRequest
                    {
                        ProductVariantId =
                            CheckoutTestDatabase.ProductVariantId,
                        Quantity = 2
                    }
                }
            });

        Assert.False(result.Succeeded);
        Assert.Equal(StoreOrderErrorCodes.InsufficientStock , result.ErrorCode);

        Assert.Equal(0 , await context.Orders.CountAsync());
    }

    [Fact]
    public async Task CreateAsync_SameIdempotencyKey_ReturnsExistingReceipt()
    {
        await using var database = new CheckoutTestDatabase();
        await database.SeedAsync();

        int orderId;

        await using ( var context = database.CreateContext() )
        {
            var service = CreateService(context);

            var first = await service.CreateAsync(
                CheckoutTestDatabase.DashboardUserId ,
                CreateRequest("idempotent-pos-sale"));

            var second = await service.CreateAsync(
                CheckoutTestDatabase.DashboardUserId ,
                CreateRequest("idempotent-pos-sale"));

            Assert.True(first.Succeeded);
            Assert.True(second.Succeeded);

            orderId = first.Response!.OrderId;

            Assert.Equal(orderId , second.Response!.OrderId);
        }

        await using var verificationContext = database.CreateContext();

        Assert.Equal(1 , await verificationContext.Orders.CountAsync());
    }

    [Fact]
    public async Task GetReceiptAsync_StoreOrder_ReturnsReceipt()
    {
        await using var database = new CheckoutTestDatabase();
        await database.SeedAsync();

        int orderId;

        await using ( var context = database.CreateContext() )
        {
            var created = await CreateService(context).CreateAsync(
                CheckoutTestDatabase.DashboardUserId ,
                CreateRequest("receipt-query"));

            Assert.True(created.Succeeded);
            orderId = created.Response!.OrderId;
        }

        await using var queryContext = database.CreateContext();

        var result = await CreateService(queryContext).GetReceiptAsync(orderId);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);
        Assert.Equal(orderId , result.Response.OrderId);
        Assert.Single(result.Response.Items);
        Assert.Equal(
            CheckoutTestDatabase.DashboardUserId ,
            result.Response.ProcessedByEmployeeId);
    }

    private static StoreOrderService CreateService(
        CheckoutTestDbContext context )
    {
        return new StoreOrderService(
            context ,
            TimeProvider.System);
    }

    private static CreateStoreOrderRequest CreateRequest(
        string idempotencyKey )
    {
        return new CreateStoreOrderRequest
        {
            PaymentMethod = PaymentMethod.CashAtStore ,
            IdempotencyKey = idempotencyKey ,
            Items = new[]
            {
                new CreateStoreOrderItemRequest
                {
                    ProductVariantId =
                        CheckoutTestDatabase.ProductVariantId,
                    Quantity = 2
                }
            }
        };
    }
}