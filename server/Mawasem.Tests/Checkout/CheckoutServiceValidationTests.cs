using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;

namespace Mawasem.Tests.Checkout;

public sealed class CheckoutServiceValidationTests
{
    [Fact]
    public async Task PreviewAsync_ValidCheckout_ReturnsCurrentTotals()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateService(dbContext);

        var result =
            await service.PreviewAsync(
                CheckoutTestDatabase.CustomerId ,
                CreateRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            200m ,
            result.Response.SubTotal);

        Assert.Equal(
            0m ,
            result.Response.Discount);

        Assert.Equal(
            25m ,
            result.Response.DeliveryFee);

        Assert.Equal(
            225m ,
            result.Response.TotalAmount);

        Assert.True(
            result.Response.CanPlaceOrder);

        Assert.Equal(
            DeliveryMethod.HomeDelivery ,
            result.Response.DeliveryMethod);

        Assert.Empty(
            result.Response.Warnings);

        var item =
            Assert.Single(
                result.Response.Items);

        Assert.Equal(
            CheckoutTestDatabase.ProductId ,
            item.ProductId);

        Assert.Equal(
            CheckoutTestDatabase.ProductVariantId ,
            item.ProductVariantId);

        Assert.Equal(
            100m ,
            item.UnitPrice);

        Assert.Equal(
            2 ,
            item.Quantity);

        Assert.Equal(
            200m ,
            item.LineTotal);
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
                        DeliveryMethod =
                            DeliveryMethod.HomeDelivery ,

                        UserAddressId =
                            null
                    });

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.AddressRequired ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_StorePickupWithoutAddress_ReturnsZeroDeliveryFee()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                DeliveryAreaActive =
                    false
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new CheckoutPreviewRequest
                    {
                        DeliveryMethod =
                            DeliveryMethod.StorePickup ,

                        UserAddressId =
                            null
                    });

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            DeliveryMethod.StorePickup ,
            result.Response.DeliveryMethod);

        Assert.Null(
            result.Response.UserAddressId);

        Assert.Null(
            result.Response.DeliveryAreaId);

        Assert.Equal(
            0m ,
            result.Response.DeliveryFee);

        Assert.Equal(
            result.Response.SubTotal ,
            result.Response.TotalAmount);
    }

    [Fact]
    public async Task PreviewAsync_InvalidDeliveryMethod_IsRejected()
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
                        DeliveryMethod =
                            (DeliveryMethod)999 ,

                        UserAddressId =
                            null
                    });

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.InvalidDeliveryMethod ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_BlockedCustomer_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CustomerBlocked =
                    true
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.CustomerBlocked ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_MissingCart_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CreateCart =
                    false
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.CartNotFound ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_EmptyCart_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                AddCartItem =
                    false
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.CartEmpty ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_UnpublishedProduct_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                ProductPublished =
                    false
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.ProductUnavailable ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_UnavailableVariant_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                VariantAvailable =
                    false
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.VariantUnavailable ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_InsufficientStock_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                StockQuantity =
                    1 ,

                CartQuantity =
                    2
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.InsufficientStock ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_PriceChanged_UsesCurrentPrice()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CurrentPrice =
                    150m ,

                PriceSnapshot =
                    100m
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            300m ,
            result.Response.SubTotal);

        Assert.Equal(
            325m ,
            result.Response.TotalAmount);

        var warning =
            Assert.Single(
                result.Response.Warnings);

        Assert.Equal(
            CheckoutWarningCodes.PriceChanged ,
            warning.Code);

        var item =
            Assert.Single(
                result.Response.Items);

        Assert.Equal(
            150m ,
            item.UnitPrice);
    }

    [Fact]
    public async Task PreviewAsync_AddressOwnedByAnotherCustomer_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                AddressBelongsToOtherCustomer =
                    true
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.AddressNotOwned ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_InactiveAddress_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                AddressActive =
                    false
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.AddressInactive ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_PendingDeliveryArea_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                DeliveryAreaStatus =
                    DeliveryAreaStatus.Pending
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.DeliveryAreaNotConfirmed ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_RestrictedDeliveryArea_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                DeliveryAreaStatus =
                    DeliveryAreaStatus.Restricted
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.DeliveryAreaNotConfirmed ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_InactiveDeliveryArea_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                DeliveryAreaActive =
                    false
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.DeliveryAreaInactive ,
            result.ErrorCode);
    }

    [Fact]
    public async Task PreviewAsync_FreeDelivery_ProducesZeroFee()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                IsFreeDelivery =
                    true
            });

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PreviewAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest());

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Response);

        Assert.Equal(
            0m ,
            result.Response.DeliveryFee);

        Assert.Equal(
            200m ,
            result.Response.TotalAmount);
    }

    [Fact]
    public async Task PreviewAsync_OnlinePayment_IsRejected()
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
                            CheckoutTestDatabase.AddressId ,

                        PaymentMethod =
                            PaymentMethod.Online
                    });

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.PaymentMethodNotSupported ,
            result.ErrorCode);
    }

    private static CheckoutPreviewRequest CreateRequest()
    {
        return new CheckoutPreviewRequest
        {
            UserAddressId =
                CheckoutTestDatabase.AddressId ,

            DeliveryMethod =
                DeliveryMethod.HomeDelivery ,

            PaymentMethod =
                PaymentMethod.CashOnDelivery
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
