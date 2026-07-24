using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Carts;
using Mawasem.Domain.Delivery;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.Checkout;

public sealed class CheckoutConcurrencyTests
{
    private const int SecondAddressId = 301;

    [Fact]
    public async Task PlaceOrderAsync_ConcurrentOrders_DoNotOversellStock()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                StockQuantity =
                    2 ,

                CartQuantity =
                    2
            });

        await SeedSecondCustomerCheckoutAsync(
            database);

        await using var firstContext =
            database.CreateContext();

        await using var secondContext =
            database.CreateContext();

        var firstService =
            new CheckoutService(
                firstContext ,
                TimeProvider.System);

        var secondService =
            new CheckoutService(
                secondContext ,
                TimeProvider.System);

        var firstTask =
            firstService.PlaceOrderAsync(
                CheckoutTestDatabase.CustomerId ,
                CreateRequest(
                    CheckoutTestDatabase.AddressId ,
                    "concurrent-first"));

        var secondTask =
            secondService.PlaceOrderAsync(
                CheckoutTestDatabase.OtherCustomerId ,
                CreateRequest(
                    SecondAddressId ,
                    "concurrent-second"));

        var results =
            await Task.WhenAll(
                firstTask ,
                secondTask);

        var successfulResult =
            Assert.Single(
                results.Where(result =>
                    result.Succeeded));

        Assert.NotNull(
            successfulResult.Response);

        var failedResult =
            Assert.Single(
                results.Where(result =>
                    !result.Succeeded));

        Assert.Contains(
            failedResult.ErrorCode ,
            new[]
            {
                CheckoutErrorCodes.InsufficientStock,
                CheckoutErrorCodes.ConcurrencyConflict,
                CheckoutErrorCodes.OrderCreationFailed
            });

        await using var verificationContext =
            database.CreateContext();

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            0 ,
            variant.StockQuantity);

        Assert.True(
            variant.StockQuantity >= 0);

        Assert.Equal(
            1 ,
            await verificationContext.Orders.CountAsync());

        Assert.Equal(
            1 ,
            await verificationContext.OrderItems.CountAsync());

        Assert.Equal(
            1 ,
            await verificationContext.CartItems.CountAsync(
                item => !item.IsDeleted));
    }

    private static async Task
        SeedSecondCustomerCheckoutAsync(
            CheckoutTestDatabase database )
    {
        await using var dbContext =
            database.CreateContext();

        var now =
            DateTimeOffset.UtcNow;

        dbContext.UserAddresses.Add(
            new UserAddress
            {
                Id =
                    SecondAddressId ,

                UserId =
                    CheckoutTestDatabase.OtherCustomerId ,

                DeliveryAreaId =
                    CheckoutTestDatabase.DeliveryAreaId ,

                Label =
                    "Home" ,

                City =
                    "Cairo" ,

                AreaName =
                    "Test Area" ,

                DetailedAddress =
                    "20 Other Street" ,

                RecipientName =
                    "Other Recipient" ,

                RecipientPhone =
                    "01000000002" ,

                IsDefault =
                    true ,

                IsActive =
                    true ,

                CreatedOn =
                    now ,

                CreatedBy =
                    "test"
            });

        var cart =
            new Cart
            {
                Id =
                    401 ,

                UserId =
                    CheckoutTestDatabase.OtherCustomerId ,

                CreatedOn =
                    now ,

                CreatedBy =
                    "test"
            };

        cart.Items.Add(
            new CartItem
            {
                Id =
                    402 ,

                ProductVariantId =
                    CheckoutTestDatabase.ProductVariantId ,

                Quantity =
                    2 ,

                UnitPriceSnapshot =
                    100m ,

                CreatedOn =
                    now ,

                CreatedBy =
                    "test"
            });

        dbContext.Carts.Add(
            cart);

        await dbContext.SaveChangesAsync();
    }

    private static PlaceOrderRequest CreateRequest(
        int userAddressId ,
        string idempotencyKey )
    {
        return new PlaceOrderRequest
        {
            UserAddressId =
                userAddressId ,

            PaymentMethod =
                PaymentMethod.CashOnDelivery ,

            IdempotencyKey =
                idempotencyKey
        };
    }
}