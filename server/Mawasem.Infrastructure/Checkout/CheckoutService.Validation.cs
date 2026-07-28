using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Carts;
using Mawasem.Domain.Delivery;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Checkout;

public sealed partial class CheckoutService
{
    private async Task<CheckoutResult<CheckoutContext>>
        LoadCheckoutContextAsync(
            int userId ,
            int? userAddressId ,
            DeliveryMethod deliveryMethod ,
            PaymentMethod paymentMethod ,
            bool trackEntities ,
            CancellationToken cancellationToken )
    {
        if ( deliveryMethod != DeliveryMethod.HomeDelivery &&
            deliveryMethod != DeliveryMethod.StorePickup )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.DeliveryMethodNotSupported ,
                "The selected delivery method is not supported.");
        }

        if ( deliveryMethod == DeliveryMethod.HomeDelivery &&
            paymentMethod != PaymentMethod.CashOnDelivery )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.PaymentMethodNotSupported ,
                "Home delivery currently supports only Cash on Delivery.");
        }

        if ( deliveryMethod == DeliveryMethod.StorePickup &&
            paymentMethod != PaymentMethod.CashAtStore )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.PaymentMethodNotSupported ,
                "Store pickup orders must initially use Cash at Store. " +
                "The final in-store payment method is recorded when collected.");
        }

        if ( deliveryMethod == DeliveryMethod.HomeDelivery &&
            ( !userAddressId.HasValue ||
                userAddressId.Value <= 0 ) )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.AddressRequired ,
                "A customer address is required for home delivery.");
        }

        IQueryable<ApplicationUser> customerQuery =
            _dbContext.Users;

        if ( !trackEntities )
        {
            customerQuery =
                customerQuery.AsNoTracking();
        }

        var customer = await customerQuery
            .SingleOrDefaultAsync(
                candidate => candidate.Id == userId ,
                cancellationToken);

        if ( customer is null )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.CustomerNotFound ,
                "The authenticated customer account was not found.");
        }

        if ( customer.IsBlocked )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.CustomerBlocked ,
                "The authenticated customer account is blocked.");
        }

        IQueryable<Cart> cartQuery =
            _dbContext.Carts
                .Include(cart => cart.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant.Product)
                .Include(cart => cart.Items)
                    .ThenInclude(item => item.ProductVariant)
                        .ThenInclude(variant => variant.Options)
                            .ThenInclude(option =>
                                option.ProductOptionValue)
                                .ThenInclude(value =>
                                    value.ProductOption)
                .AsSplitQuery();

        if ( !trackEntities )
        {
            cartQuery =
                cartQuery.AsNoTrackingWithIdentityResolution();
        }

        var cart = await cartQuery
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.UserId == userId &&
                    !candidate.IsDeleted ,
                cancellationToken);

        if ( cart is null )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.CartNotFound ,
                "The customer cart was not found.");
        }

        var activeItems = cart.Items
            .Where(item => !item.IsDeleted)
            .OrderBy(item => item.Id)
            .ToArray();

        if ( activeItems.Length == 0 )
        {
            return CheckoutResult<CheckoutContext>.Failure(
                CheckoutErrorCodes.CartEmpty ,
                "The customer cart is empty.");
        }

        var lines = new List<CheckoutLine>(
            activeItems.Length);

        foreach ( var cartItem in activeItems )
        {
            var variant = cartItem.ProductVariant;
            var product = variant.Product;

            if ( product.IsDeleted ||
                !product.IsPublished )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.ProductUnavailable ,
                    $"Product '{product.Name.English}' is not available.");
            }

            if ( variant.IsDeleted ||
                !variant.IsAvailable )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.VariantUnavailable ,
                    $"Product variant '{variant.SKU}' is not available.");
            }

            if ( variant.StockQuantity < cartItem.Quantity )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.InsufficientStock ,
                    $"Only {variant.StockQuantity} unit(s) of " +
                    $"'{variant.SKU}' are currently available.");
            }

            var summaries =
                CreateVariantSummaries(variant);

            var unitPrice =
                product.CurrentPrice;

            lines.Add(
                new CheckoutLine
                {
                    CartItem = cartItem ,
                    Product = product ,
                    Variant = variant ,
                    VariantSummaryAr = summaries.Arabic ,
                    VariantSummaryEn = summaries.English ,
                    UnitPrice = unitPrice ,
                    LineTotal = CalculateLineTotal(
                        unitPrice ,
                        cartItem.Quantity)
                });
        }

        UserAddress? address = null;

        decimal deliveryFee = 0m;

        if ( deliveryMethod == DeliveryMethod.HomeDelivery )
        {
            var addressQuery =
                _dbContext.UserAddresses
                    .Include(candidate =>
                        candidate.DeliveryArea)
                    .AsQueryable();

            if ( !trackEntities )
            {
                addressQuery =
                    addressQuery.AsNoTracking();
            }

            address = await addressQuery
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == userAddressId!.Value &&
                        !candidate.IsDeleted ,
                    cancellationToken);

            if ( address is null )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.AddressNotFound ,
                    "The selected customer address was not found.");
            }

            if ( address.UserId != userId )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.AddressNotOwned ,
                    "The selected address does not belong to the customer.");
            }

            if ( !address.IsActive )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.AddressInactive ,
                    "The selected customer address is inactive.");
            }

            var deliveryArea =
                address.DeliveryArea;

            if ( deliveryArea is null ||
                deliveryArea.IsDeleted )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.DeliveryAreaNotFound ,
                    "The delivery area associated with the address was not found.");
            }

            if ( !deliveryArea.IsActive )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.DeliveryAreaInactive ,
                    "The delivery area associated with the address is inactive.");
            }

            if ( deliveryArea.Status !=
                DeliveryAreaStatus.Confirmed )
            {
                return CheckoutResult<CheckoutContext>.Failure(
                    CheckoutErrorCodes.DeliveryAreaNotConfirmed ,
                    "Checkout is available only for confirmed delivery areas.");
            }

            deliveryFee =
                deliveryArea.IsFreeDelivery
                    ? 0m
                    : deliveryArea.DeliveryFee;
        }

        var subTotal = decimal.Round(
            lines.Sum(line => line.LineTotal) ,
            2 ,
            MidpointRounding.AwayFromZero);

        const decimal discount = 0m;

        var totalAmount = decimal.Round(
            subTotal - discount + deliveryFee ,
            2 ,
            MidpointRounding.AwayFromZero);

        return CheckoutResult<CheckoutContext>.Success(
            new CheckoutContext
            {
                Customer = customer ,
                Cart = cart ,
                Address = address ,
                DeliveryMethod = deliveryMethod ,
                PaymentMethod = paymentMethod ,
                Lines = lines ,
                SubTotal = subTotal ,
                Discount = discount ,
                DeliveryFee = deliveryFee ,
                TotalAmount = totalAmount
            });
    }
}