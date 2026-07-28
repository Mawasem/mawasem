namespace Mawasem.Application.Features.Checkout.Models;

public static class CheckoutErrorCodes
{
    public const string CustomerNotFound =
        "checkout.customer_not_found";

    public const string CustomerBlocked =
        "checkout.customer_blocked";

    public const string CartNotFound =
        "checkout.cart_not_found";

    public const string CartEmpty =
        "checkout.cart_empty";

    public const string ProductUnavailable =
        "checkout.product_unavailable";

    public const string VariantUnavailable =
        "checkout.variant_unavailable";

    public const string InsufficientStock =
        "checkout.insufficient_stock";

    public const string DeliveryMethodNotSupported =
        "checkout.delivery_method_not_supported";

    public const string AddressRequired =
        "checkout.address_required";

    public const string AddressNotFound =
        "checkout.address_not_found";

    public const string AddressNotOwned =
        "checkout.address_not_owned";

    public const string AddressInactive =
        "checkout.address_inactive";

    public const string DeliveryAreaNotFound =
        "checkout.delivery_area_not_found";

    public const string DeliveryAreaInactive =
        "checkout.delivery_area_inactive";

    public const string DeliveryAreaNotConfirmed =
        "checkout.delivery_area_not_confirmed";

    public const string PaymentMethodNotSupported =
        "checkout.payment_method_not_supported";

    public const string InvalidIdempotencyKey =
        "checkout.invalid_idempotency_key";

    public const string InvalidNotes =
        "checkout.invalid_notes";

    public const string ConcurrencyConflict =
        "checkout.concurrency_conflict";

    public const string OrderCreationFailed =
        "checkout.order_creation_failed";
}