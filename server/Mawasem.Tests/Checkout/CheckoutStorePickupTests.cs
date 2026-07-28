using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.Checkout;

public sealed class CheckoutStorePickupTests
{
    [Fact]
    public async Task PreviewAsync_StorePickup_ReturnsZeroDeliveryFeeWithoutAddress()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreatePickupPreviewRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Null(
            result.Response.UserAddressId);

        Assert.Null(
            result.Response.DeliveryAreaId);

        Assert.Equal(
            DeliveryMethod.StorePickup ,
            result.Response.DeliveryMethod);

        Assert.Equal(
            PaymentMethod.CashAtStore ,
            result.Response.PaymentMethod);

        Assert.Equal(
            200m ,
            result.Response.SubTotal);

        Assert.Equal(
            0m ,
            result.Response.DeliveryFee);

        Assert.Equal(
            200m ,
            result.Response.TotalAmount);

        Assert.True(
            result.Response.CanPlaceOrder);

        Assert.Single(
            result.Response.Items);
    }

    [Fact]
    public async Task PlaceOrderAsync_StorePickup_CreatesPendingWebsiteOrder()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        int orderId;

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateService(dbContext)
                    .PlaceOrderAsync(
                        CheckoutTestDatabase.CustomerId ,
                        CreatePickupOrderRequest(
                            "store-pickup-order"));

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Response);

            Assert.Equal(
                DeliveryMethod.StorePickup ,
                result.Response.DeliveryMethod);

            Assert.Equal(
                PaymentMethod.CashAtStore ,
                result.Response.PaymentMethod);

            Assert.Equal(
                OrderStatus.Pending ,
                result.Response.OrderStatus);

            Assert.Equal(
                PaymentStatus.Pending ,
                result.Response.PaymentStatus);

            Assert.Equal(
                0m ,
                result.Response.DeliveryFee);

            Assert.Equal(
                200m ,
                result.Response.TotalAmount);

            orderId =
                result.Response.OrderId;
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        Assert.Equal(
            CheckoutTestDatabase.CustomerId ,
            order.UserId);

        Assert.Equal(
            OrderSource.Website ,
            order.OrderSource);

        Assert.Equal(
            DeliveryMethod.StorePickup ,
            order.DeliveryMethod);

        Assert.Equal(
            PaymentMethod.CashAtStore ,
            order.PaymentMethod);

        Assert.Equal(
            OrderStatus.Pending ,
            order.OrderStatus);

        Assert.Equal(
            PaymentStatus.Pending ,
            order.PaymentStatus);

        Assert.Null(
            order.UserAddressId);

        Assert.Null(
            order.ShippingDeliveryAreaId);

        Assert.Null(
            order.ShippingRecipientName);

        Assert.Null(
            order.ShippingRecipientPhone);

        Assert.Null(
            order.ShippingDetailedAddress);

        Assert.Equal(
            0m ,
            order.DeliveryFee);

        Assert.Single(
            order.OrderItems);
    }

    [Fact]
    public async Task PlaceOrderAsync_StorePickup_DeductsSharedStockAndClearsCart()
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

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateService(dbContext)
                    .PlaceOrderAsync(
                        CheckoutTestDatabase.CustomerId ,
                        CreatePickupOrderRequest(
                            "pickup-stock-reservation"));

            Assert.True(result.Succeeded);
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

        var cartItem =
            await verificationContext.CartItems
                .SingleAsync();

        Assert.True(
            cartItem.IsDeleted);

        Assert.NotNull(
            cartItem.DeletedOn);

        Assert.Equal(
            1 ,
            await verificationContext.Orders.CountAsync());
    }

    [Fact]
    public async Task PreviewAsync_HomeDeliveryWithoutAddress_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new CheckoutPreviewRequest
                    {
                        UserAddressId =
                            null ,

                        DeliveryMethod =
                            DeliveryMethod.HomeDelivery ,

                        PaymentMethod =
                            PaymentMethod.CashOnDelivery
                    });

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.AddressRequired ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_StorePickupWithCashOnDelivery_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new CheckoutPreviewRequest
                    {
                        UserAddressId =
                            null ,

                        DeliveryMethod =
                            DeliveryMethod.StorePickup ,

                        PaymentMethod =
                            PaymentMethod.CashOnDelivery
                    });

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.PaymentMethodNotSupported ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_StorePickup_DoesNotRequireActiveDeliveryArea()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                AddressActive =
                    false ,

                DeliveryAreaActive =
                    false ,

                DeliveryAreaStatus =
                    DeliveryAreaStatus.Restricted
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreatePickupPreviewRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Null(
            result.Response.UserAddressId);

        Assert.Equal(
            0m ,
            result.Response.DeliveryFee);
    }

    private static CheckoutPreviewRequest
        CreatePickupPreviewRequest()
    {
        return new CheckoutPreviewRequest
        {
            UserAddressId =
                null ,

            DeliveryMethod =
                DeliveryMethod.StorePickup ,

            PaymentMethod =
                PaymentMethod.CashAtStore
        };
    }

    private static PlaceOrderRequest
        CreatePickupOrderRequest(
            string idempotencyKey )
    {
        return new PlaceOrderRequest
        {
            UserAddressId =
                null ,

            DeliveryMethod =
                DeliveryMethod.StorePickup ,

            PaymentMethod =
                PaymentMethod.CashAtStore ,

            Notes =
                "Customer will collect from the store." ,

            IdempotencyKey =
                idempotencyKey
        };
    }

    private static CheckoutService CreateService(
        CheckoutTestDbContext dbContext )
    {
        return new CheckoutService(
            dbContext ,
            TimeProvider.System);
    }
}