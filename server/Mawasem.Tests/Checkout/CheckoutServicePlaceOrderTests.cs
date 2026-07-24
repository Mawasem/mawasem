using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Checkout;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Mawasem.Tests.Checkout;

public sealed class CheckoutServicePlaceOrderTests
{
    [Fact]
    public async Task PlaceOrderAsync_ValidCheckout_CreatesPendingCodOrder()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        int createdOrderId;

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateService(dbContext)
                    .PlaceOrderAsync(
                        CheckoutTestDatabase.CustomerId ,
                        CreateRequest(
                            "valid-order"));

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Response);
            Assert.False(
                result.Response.IsIdempotentReplay);

            Assert.Equal(
                OrderStatus.Pending ,
                result.Response.OrderStatus);

            Assert.Equal(
                PaymentStatus.Pending ,
                result.Response.PaymentStatus);

            Assert.Equal(
                PaymentMethod.CashOnDelivery ,
                result.Response.PaymentMethod);

            Assert.Equal(
                DeliveryMethod.HomeDelivery ,
                result.Response.DeliveryMethod);

            Assert.Equal(
                200m ,
                result.Response.SubTotal);

            Assert.Equal(
                25m ,
                result.Response.DeliveryFee);

            Assert.Equal(
                225m ,
                result.Response.TotalAmount);

            Assert.Matches(
                new Regex(
                    "^MWS-[0-9]{8}-[0-9A-F]{8}$") ,

                result.Response.OrderNumber);

            createdOrderId =
                result.Response.OrderId;
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id == createdOrderId);

        Assert.Equal(
            "valid-order" ,
            order.IdempotencyKey);

        Assert.Equal(
            OrderStatus.Pending ,
            order.OrderStatus);

        Assert.Equal(
            PaymentStatus.Pending ,
            order.PaymentStatus);

        Assert.Equal(
            PaymentMethod.CashOnDelivery ,
            order.PaymentMethod);

        Assert.Null(
            order.CouponCode);

        Assert.Equal(
            0m ,
            order.Discount);

        Assert.Single(
            order.OrderItems);
    }

    [Fact]
    public async Task PlaceOrderAsync_Success_DeductsStockAndClearsCart()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                StockQuantity =
                    10 ,

                CartQuantity =
                    2
            });

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateService(dbContext)
                    .PlaceOrderAsync(
                        CheckoutTestDatabase.CustomerId ,
                        CreateRequest(
                            "stock-deduction"));

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
            8 ,
            variant.StockQuantity);

        var cartItem =
            await verificationContext.CartItems
                .SingleAsync();

        Assert.True(
            cartItem.IsDeleted);

        Assert.NotNull(
            cartItem.DeletedOn);

        var cart =
            await verificationContext.Carts
                .SingleAsync();

        Assert.False(
            cart.IsDeleted);
    }

    [Fact]
    public async Task PlaceOrderAsync_UsesCurrentPrice()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync(
            new CheckoutSeedOptions
            {
                CurrentPrice =
                    125m ,

                PriceSnapshot =
                    80m ,

                CartQuantity =
                    2
            });

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateService(dbContext)
                    .PlaceOrderAsync(
                        CheckoutTestDatabase.CustomerId ,
                        CreateRequest(
                            "current-price"));

            Assert.True(result.Succeeded);
            Assert.NotNull(result.Response);

            Assert.Equal(
                250m ,
                result.Response.SubTotal);

            Assert.Equal(
                275m ,
                result.Response.TotalAmount);
        }

        await using var verificationContext =
            database.CreateContext();

        var orderItem =
            await verificationContext.OrderItems
                .SingleAsync();

        Assert.Equal(
            125m ,
            orderItem.UnitPrice);

        Assert.Equal(
            250m ,
            orderItem.TotalPrice);
    }

    [Fact]
    public async Task PlaceOrderAsync_CopiesImmutableSnapshots()
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
                        CreateRequest(
                            "snapshot-order"));

            Assert.True(result.Succeeded);

            orderId =
                result.Response!.OrderId;
        }

        await using (
            var mutationContext =
                database.CreateContext() )
        {
            var product =
                await mutationContext.Products
                    .SingleAsync();

            product.Name.Update(
                "Changed Product" ,
                "منتج متغير");

            var address =
                await mutationContext.UserAddresses
                    .SingleAsync();

            address.RecipientName =
                "Changed Recipient";

            address.DetailedAddress =
                "Changed Address";

            var deliveryArea =
                await mutationContext.DeliveryAreas
                    .SingleAsync();

            deliveryArea.Name.Update(
                "Changed Area" ,
                "منطقة متغيرة");

            await mutationContext.SaveChangesAsync();
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
            "Test Recipient" ,
            order.ShippingRecipientName);

        Assert.Equal(
            "10 Test Street" ,
            order.ShippingDetailedAddress);

        Assert.Equal(
            "Test Area" ,
            order.ShippingDeliveryAreaNameEn);

        Assert.Equal(
            "منطقة اختبار" ,
            order.ShippingDeliveryAreaNameAr);

        var orderItem =
            Assert.Single(
                order.OrderItems);

        Assert.Equal(
            "Test Product" ,
            orderItem.ProductNameEn);

        Assert.Equal(
            "منتج اختبار" ,
            orderItem.ProductNameAr);

        Assert.Equal(
            "TEST-101" ,
            orderItem.SKU);
    }

    [Fact]
    public async Task PlaceOrderAsync_DuplicateIdempotencyKey_ReturnsExistingOrder()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateService(dbContext);

        var firstResult =
            await service.PlaceOrderAsync(
                CheckoutTestDatabase.CustomerId ,
                CreateRequest(
                    "duplicate-key"));

        var secondResult =
            await service.PlaceOrderAsync(
                CheckoutTestDatabase.CustomerId ,
                CreateRequest(
                    "duplicate-key"));

        Assert.True(
            firstResult.Succeeded);

        Assert.True(
            secondResult.Succeeded);

        Assert.False(
            firstResult.Response!.IsIdempotentReplay);

        Assert.True(
            secondResult.Response!.IsIdempotentReplay);

        Assert.Equal(
            firstResult.Response.OrderId ,
            secondResult.Response.OrderId);

        Assert.Equal(
            firstResult.Response.OrderNumber ,
            secondResult.Response.OrderNumber);

        await using var verificationContext =
            database.CreateContext();

        Assert.Equal(
            1 ,
            await verificationContext.Orders.CountAsync());

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            8 ,
            variant.StockQuantity);
    }

    [Fact]
    public async Task PlaceOrderAsync_InvalidIdempotencyKey_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await CreateService(dbContext)
                .PlaceOrderAsync(
                    CheckoutTestDatabase.CustomerId ,
                    CreateRequest("   "));

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.InvalidIdempotencyKey ,
            result.ErrorCode);

        Assert.Empty(
            dbContext.Orders);
    }

    [Fact]
    public async Task PlaceOrderAsync_OnlinePayment_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var request =
            CreateRequest(
                "online-payment") with
            {
                PaymentMethod =
                    PaymentMethod.Online
            };

        var result =
            await CreateService(dbContext)
                .PlaceOrderAsync(
                    CheckoutTestDatabase.CustomerId ,
                    request);

        Assert.False(result.Succeeded);

        Assert.Equal(
            CheckoutErrorCodes.PaymentMethodNotSupported ,
            result.ErrorCode);

        Assert.Empty(
            dbContext.Orders);
    }

    [Fact]
    public async Task PlaceOrderAsync_InsufficientStock_ChangesNothing()
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

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var result =
                await CreateService(dbContext)
                    .PlaceOrderAsync(
                        CheckoutTestDatabase.CustomerId ,
                        CreateRequest(
                            "insufficient-stock"));

            Assert.False(result.Succeeded);

            Assert.Equal(
                CheckoutErrorCodes.InsufficientStock ,
                result.ErrorCode);
        }

        await using var verificationContext =
            database.CreateContext();

        Assert.Empty(
            verificationContext.Orders);

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            1 ,
            variant.StockQuantity);

        var cartItem =
            await verificationContext.CartItems
                .SingleAsync();

        Assert.False(
            cartItem.IsDeleted);
    }

    [Fact]
    public async Task PlaceOrderAsync_DatabaseFailure_RollsBackEverything()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using (
            var dbContext =
                database.CreateContext() )
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                CREATE TRIGGER FailOrderInsert
                BEFORE INSERT ON Orders
                BEGIN
                    SELECT RAISE(
                        ABORT,
                        'Forced order insert failure');
                END;
                """);

            var result =
                await CreateService(dbContext)
                    .PlaceOrderAsync(
                        CheckoutTestDatabase.CustomerId ,
                        CreateRequest(
                            "forced-failure"));

            Assert.False(result.Succeeded);

            Assert.Equal(
                CheckoutErrorCodes.OrderCreationFailed ,
                result.ErrorCode);
        }

        await using var verificationContext =
            database.CreateContext();

        Assert.Empty(
            verificationContext.Orders);

        Assert.Empty(
            verificationContext.OrderItems);

        var variant =
            await verificationContext.ProductVariants
                .SingleAsync();

        Assert.Equal(
            10 ,
            variant.StockQuantity);

        var cartItem =
            await verificationContext.CartItems
                .SingleAsync();

        Assert.False(
            cartItem.IsDeleted);
    }

    private static PlaceOrderRequest CreateRequest(
        string idempotencyKey )
    {
        return new PlaceOrderRequest
        {
            UserAddressId =
                CheckoutTestDatabase.AddressId ,

            PaymentMethod =
                PaymentMethod.CashOnDelivery ,

            Notes =
                "Test order" ,

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