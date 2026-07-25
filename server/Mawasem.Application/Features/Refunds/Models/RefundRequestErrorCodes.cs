namespace Mawasem.Application.Features.Refunds.Models;

public static class RefundRequestErrorCodes
{
    public const string InvalidRequest =
        "refunds.invalid_request";

    public const string CustomerNotFound =
        "refunds.customer_not_found";

    public const string CustomerBlocked =
        "refunds.customer_blocked";

    public const string OrderNotFound =
        "refunds.order_not_found";

    public const string OrderAccessDenied =
        "refunds.order_access_denied";

    public const string OrderNotDelivered =
        "refunds.order_not_delivered";

    public const string RefundRequestNotFound =
        "refunds.refund_request_not_found";

    public const string RefundRequestAccessDenied =
        "refunds.refund_request_access_denied";

    public const string InvalidIdempotencyKey =
        "refunds.invalid_idempotency_key";

    public const string InvalidCustomerReason =
        "refunds.invalid_customer_reason";

    public const string ItemsRequired =
        "refunds.items_required";

    public const string DuplicateOrderItem =
        "refunds.duplicate_order_item";

    public const string OrderItemNotFound =
        "refunds.order_item_not_found";

    public const string InvalidQuantity =
        "refunds.invalid_quantity";

    public const string QuantityExceedsRefundable =
        "refunds.quantity_exceeds_refundable";

    public const string InvalidStatusTransition =
        "refunds.invalid_status_transition";

    public const string InvalidAdminNotes =
        "refunds.invalid_admin_notes";

    public const string ConcurrencyConflict =
        "refunds.concurrency_conflict";

    public const string OperationFailed =
        "refunds.operation_failed";
}