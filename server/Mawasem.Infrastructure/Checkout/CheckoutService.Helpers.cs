using Mawasem.Application.Features.Checkout.Contracts.Responses;
using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Common;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using System.Globalization;
using System.Security.Cryptography;

namespace Mawasem.Infrastructure.Checkout;

public sealed partial class CheckoutService
{
    private static CheckoutPreviewResponse
        CreatePreviewResponse(
            CheckoutContext checkoutContext ,
            PaymentMethod paymentMethod )
    {
        var items = checkoutContext.Lines
            .Select(
                line =>
                    new CheckoutItemResponse
                    {
                        CartItemId =
                            line.CartItem.Id ,

                        ProductId =
                            line.Product.Id ,

                        ProductVariantId =
                            line.Variant.Id ,

                        ProductNameAr =
                            line.Product.Name.Arabic ,

                        ProductNameEn =
                            line.Product.Name.English ,

                        Sku =
                            line.Variant.SKU ,

                        VariantSummaryAr =
                            line.VariantSummaryAr ,

                        VariantSummaryEn =
                            line.VariantSummaryEn ,

                        UnitPrice =
                            line.UnitPrice ,

                        Quantity =
                            line.CartItem.Quantity ,

                        LineTotal =
                            line.LineTotal
                    })
            .ToArray();

        var warnings = checkoutContext.Lines
            .Where(line =>
                line.CartItem.UnitPriceSnapshot !=
                line.UnitPrice)
            .Select(
                line =>
                    new CheckoutWarningResponse
                    {
                        Code =
                            CheckoutWarningCodes.PriceChanged ,

                        Message =
                            $"The price of product variant " +
                            $"'{line.Variant.SKU}' has changed. " +
                            $"The current price will be used."
                    })
            .ToArray();

        return new CheckoutPreviewResponse
        {
            CartId =
                checkoutContext.Cart.Id ,

            UserAddressId =
                checkoutContext.Address?.Id ,

            DeliveryAreaId =
                checkoutContext.Address?.DeliveryAreaId ,

            Items =
                items ,

            SubTotal =
                checkoutContext.SubTotal ,

            Discount =
                checkoutContext.Discount ,

            DeliveryFee =
                checkoutContext.DeliveryFee ,

            TotalAmount =
                checkoutContext.TotalAmount ,

            PaymentMethod =
                paymentMethod ,

            DeliveryMethod =
                checkoutContext.DeliveryMethod ,

            CanPlaceOrder =
                true ,

            Warnings =
                warnings
        };
    }

    private static Order CreateOrder(
        CheckoutContext checkoutContext ,
        string idempotencyKey ,
        string? notes ,
        DateTimeOffset now ,
        string actor )
    {
        var customer =
            checkoutContext.Customer;

        var address =
            checkoutContext.Address;

        var deliveryArea =
            address?.DeliveryArea;

        var order = new Order
        {
            UserId =
                customer.Id ,

            UserAddressId =
                address?.Id ,

            ShippingDeliveryAreaId =
                deliveryArea?.Id ,

            CustomerNameAr =
                customer.FullNameAr ,

            CustomerNameEn =
                customer.FullNameEn ,

            CustomerPhone =
                customer.PhoneNumber
                ?? string.Empty ,

            ShippingRecipientName =
                address?.RecipientName ,

            ShippingRecipientPhone =
                address?.RecipientPhone ,

            ShippingCity =
                address?.City ,

            ShippingAreaName =
                address?.AreaName ,

            ShippingDetailedAddress =
                address?.DetailedAddress ,

            ShippingBuildingNumber =
                address?.BuildingNumber ,

            ShippingFloorNumber =
                address?.FloorNumber ,

            ShippingApartmentNumber =
                address?.ApartmentNumber ,

            ShippingLandmark =
                address?.Landmark ,

            ShippingDeliveryAreaNameAr =
                deliveryArea?.Name.Arabic ,

            ShippingDeliveryAreaNameEn =
                deliveryArea?.Name.English ,

            OrderNumber =
                CreateOrderNumber(now) ,

            OrderDate =
                now.UtcDateTime ,

            IdempotencyKey =
                idempotencyKey ,

            SubTotal =
                checkoutContext.SubTotal ,

            Discount =
                checkoutContext.Discount ,

            DeliveryFee =
                checkoutContext.DeliveryFee ,

            TotalAmount =
                checkoutContext.TotalAmount ,

            CouponCode =
                null ,

            OrderStatus =
                OrderStatus.Pending ,

            PaymentMethod =
                PaymentMethod.CashOnDelivery ,

            PaymentStatus =
                PaymentStatus.Pending ,

            DeliveryMethod =
                checkoutContext.DeliveryMethod ,

            OrderSource =
                OrderSource.Website ,

            Notes =
                notes ,

            CreatedOn =
                now ,

            CreatedBy =
                actor
        };

        foreach ( var line in checkoutContext.Lines )
        {
            order.OrderItems.Add(
                new OrderItem
                {
                    ProductId =
                        line.Product.Id ,

                    ProductVariantId =
                        line.Variant.Id ,

                    ProductNameAr =
                        line.Product.Name.Arabic ,

                    ProductNameEn =
                        line.Product.Name.English ,

                    SKU =
                        line.Variant.SKU ,

                    VariantSummaryAr =
                        line.VariantSummaryAr ,

                    VariantSummaryEn =
                        line.VariantSummaryEn ,

                    UnitPrice =
                        line.UnitPrice ,

                    DiscountAmount =
                        0m ,

                    Quantity =
                        line.CartItem.Quantity ,

                    TotalPrice =
                        line.LineTotal ,

                    RefundedQuantity =
                        0 ,

                    CreatedOn =
                        now ,

                    CreatedBy =
                        actor
                });
        }

        return order;
    }

    private static PlaceOrderResponse
        CreatePlaceOrderResponse(
            Order order ,
            bool isIdempotentReplay )
    {
        return new PlaceOrderResponse
        {
            OrderId =
                order.Id ,

            OrderNumber =
                order.OrderNumber ,

            OrderDate =
                order.OrderDate ,

            OrderStatus =
                order.OrderStatus ,

            PaymentStatus =
                order.PaymentStatus ,

            PaymentMethod =
                order.PaymentMethod ,

            DeliveryMethod =
                order.DeliveryMethod ,

            SubTotal =
                order.SubTotal ,

            Discount =
                order.Discount ,

            DeliveryFee =
                order.DeliveryFee ,

            TotalAmount =
                order.TotalAmount ,

            IsIdempotentReplay =
                isIdempotentReplay
        };
    }

    private static (
        string Arabic ,
        string English)
        CreateVariantSummaries(
            ProductVariant variant )
    {
        var options = variant.Options
            .Where(option =>
                !option.IsDeleted &&
                !option.ProductOptionValue.IsDeleted &&
                !option.ProductOptionValue
                    .ProductOption.IsDeleted)
            .OrderBy(option =>
                option.ProductOptionValue.ProductOptionId)
            .ThenBy(option =>
                option.ProductOptionValueId)
            .ToArray();

        var arabic = string.Join(
            "، " ,
            options.Select(
                option =>
                    CreateOptionSummary(
                        option.ProductOptionValue
                            .ProductOption.Name.Arabic ,

                        option.ProductOptionValue
                            .Value.Arabic)));

        var english = string.Join(
            ", " ,
            options.Select(
                option =>
                    CreateOptionSummary(
                        option.ProductOptionValue
                            .ProductOption.Name.English ,

                        option.ProductOptionValue
                            .Value.English)));

        return (
            arabic ,
            english);
    }

    private static string CreateOptionSummary(
        string optionName ,
        string optionValue )
    {
        var normalizedName =
            optionName.Trim();

        var normalizedValue =
            optionValue.Trim();

        if ( normalizedName.Length == 0 )
        {
            return normalizedValue;
        }

        if ( normalizedValue.Length == 0 )
        {
            return normalizedName;
        }

        return $"{normalizedName}: {normalizedValue}";
    }

    private static string CreateOrderNumber(
        DateTimeOffset now )
    {
        var randomSuffix =
            Convert.ToHexString(
                RandomNumberGenerator.GetBytes(4));

        return
            $"MWS-{now.UtcDateTime:yyyyMMdd}-{randomSuffix}";
    }

    private static decimal CalculateLineTotal(
        decimal unitPrice ,
        int quantity )
    {
        return decimal.Round(
            unitPrice * quantity ,
            2 ,
            MidpointRounding.AwayFromZero);
    }

    private static string GetCustomerActor(
        int userId )
    {
        return userId.ToString(
            CultureInfo.InvariantCulture);
    }

    private static string? NormalizeOptionalText(
        string? value )
    {
        if ( string.IsNullOrWhiteSpace(value) )
        {
            return null;
        }

        return value.Trim();
    }

    private static void MarkModified(
        BaseAuditableEntity entity ,
        DateTimeOffset now ,
        string actor )
    {
        entity.LastModifiedOn =
            now;

        entity.LastModifiedBy =
            actor;
    }

    private static void MarkDeleted(
        BaseAuditableEntity entity ,
        DateTimeOffset now ,
        string actor )
    {
        entity.IsDeleted =
            true;

        entity.DeletedOn =
            now;

        entity.DeletedBy =
            actor;

        MarkModified(
            entity ,
            now ,
            actor);
    }
}
