using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreReturns.Contracts.Requests;
using Mawasem.Application.Features.StoreReturns.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.StoreOrders;
using Mawasem.Infrastructure.StoreReturns;
using Mawasem.Tests.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.StoreReturns;

public sealed class StoreReturnServiceTests
{
    [Fact]
    public async Task CreateAsync_FullReturn_RestoresStockAndRefundsOrder()
    {
        await using var database = new CheckoutTestDatabase();
        await database.SeedAsync(new CheckoutSeedOptions { StockQuantity = 10 });

        var (orderId , orderItemId) = await CreateSaleAsync(database , "full-return");

        await using ( var context = database.CreateContext() )
        {
            var result = await CreateReturnService(context).CreateAsync(
                CheckoutTestDatabase.DashboardUserId ,
                orderId ,
                CreateReturnRequest(orderItemId , 2));

            Assert.True(
                result.Succeeded ,
                $"{result.ErrorCode}: {result.ErrorMessage}");

            Assert.Equal(OrderStatus.Refunded , result.Response!.OrderStatus);
            Assert.Equal(200m , result.Response.TotalRefundAmount);
        }

        await using var verificationContext = database.CreateContext();

        var variant = await verificationContext.ProductVariants.SingleAsync(
            item => item.Id == CheckoutTestDatabase.ProductVariantId);

        var order = await verificationContext.Orders
            .Include(item => item.OrderItems)
            .SingleAsync(item => item.Id == orderId);

        Assert.Equal(10 , variant.StockQuantity);
        Assert.Equal(OrderStatus.Refunded , order.OrderStatus);
        Assert.Equal(PaymentStatus.Refunded , order.PaymentStatus);
        Assert.Equal(2 , order.OrderItems.Single().RefundedQuantity);
    }

    [Fact]
    public async Task CreateAsync_PartialReturn_RestoresOnlyReturnedQuantity()
    {
        await using var database = new CheckoutTestDatabase();
        await database.SeedAsync(new CheckoutSeedOptions { StockQuantity = 10 });

        var (orderId , orderItemId) = await CreateSaleAsync(database , "partial-return");

        await using ( var context = database.CreateContext() )
        {
            var result = await CreateReturnService(context).CreateAsync(
                CheckoutTestDatabase.DashboardUserId ,
                orderId ,
                CreateReturnRequest(orderItemId , 1));

            Assert.True(
                result.Succeeded ,
                $"{result.ErrorCode}: {result.ErrorMessage}");

            Assert.Equal(
                OrderStatus.PartiallyRefunded ,
                result.Response!.OrderStatus);
        }

        await using var verificationContext = database.CreateContext();

        var variant = await verificationContext.ProductVariants.SingleAsync(
            item => item.Id == CheckoutTestDatabase.ProductVariantId);

        Assert.Equal(9 , variant.StockQuantity);
    }

    [Fact]
    public async Task CreateAsync_QuantityExceedsRemainingReturnable_Fails()
    {
        await using var database = new CheckoutTestDatabase();
        await database.SeedAsync();

        var (orderId , orderItemId) = await CreateSaleAsync(database , "excess-return");

        await using var context = database.CreateContext();

        var result = await CreateReturnService(context).CreateAsync(
            CheckoutTestDatabase.DashboardUserId ,
            orderId ,
            CreateReturnRequest(orderItemId , 3));

        Assert.False(result.Succeeded);
        Assert.Equal(
            StoreReturnErrorCodes.QuantityExceedsReturnable ,
            result.ErrorCode);
    }

    private static async Task<(int OrderId , int OrderItemId)> CreateSaleAsync(
        CheckoutTestDatabase database ,
        string idempotencyKey )
    {
        await using var context = database.CreateContext();

        var sale = await CreateOrderService(context).CreateAsync(
            CheckoutTestDatabase.DashboardUserId ,
            new CreateStoreOrderRequest
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
            });

        Assert.True(sale.Succeeded);

        var orderItemId = await context.OrderItems
            .Where(item => item.OrderId == sale.Response!.OrderId)
            .Select(item => item.Id)
            .SingleAsync();

        return (sale.Response!.OrderId , orderItemId);
    }

    private static StoreOrderService CreateOrderService(
        CheckoutTestDbContext context ) =>
        new(context , TimeProvider.System);

    private static StoreReturnService CreateReturnService(
        CheckoutTestDbContext context ) =>
        new(context , TimeProvider.System);

    private static CreateStoreReturnRequest CreateReturnRequest(
        int orderItemId ,
        int quantity ) =>
        new()
        {
            RefundPaymentMethod = PaymentMethod.CashAtStore ,
            Items = new[]
            {
                new CreateStoreReturnItemRequest
                {
                    OrderItemId = orderItemId,
                    Quantity = quantity
                }
            }
        };
}