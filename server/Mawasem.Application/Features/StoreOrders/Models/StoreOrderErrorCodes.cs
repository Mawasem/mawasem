namespace Mawasem.Application.Features.StoreOrders.Models;

public static class StoreOrderErrorCodes
{
    public const string InvalidRequest =
        "store_orders.invalid_request";

    public const string InvalidEmployee =
        "store_orders.invalid_employee";

    public const string StoreEmployeeNotFound =
        "store_orders.employee_not_found";

    public const string StoreEmployeeBlocked =
        "store_orders.employee_blocked";

    public const string InvalidIdempotencyKey =
        "store_orders.invalid_idempotency_key";

    public const string ItemsRequired =
        "store_orders.items_required";

    public const string InvalidVariant =
        "store_orders.invalid_variant";

    public const string DuplicateVariant =
        "store_orders.duplicate_variant";

    public const string InvalidQuantity =
        "store_orders.invalid_quantity";

    public const string ProductNotFound =
        "store_orders.product_not_found";

    public const string ProductUnavailable =
        "store_orders.product_unavailable";

    public const string VariantUnavailable =
        "store_orders.variant_unavailable";

    public const string InsufficientStock =
        "store_orders.insufficient_stock";

    public const string InvalidPaymentMethod =
        "store_orders.invalid_payment_method";

    public const string PaymentReferenceRequired =
        "store_orders.payment_reference_required";

    public const string InvalidPaymentReference =
        "store_orders.invalid_payment_reference";

    public const string InvalidNotes =
        "store_orders.invalid_notes";

    public const string OrderNotFound =
        "store_orders.order_not_found";

    public const string ReceiptAccessDenied =
        "store_orders.receipt_access_denied";

    public const string ConcurrencyConflict =
        "store_orders.concurrency_conflict";

    public const string OperationFailed =
        "store_orders.operation_failed";
}