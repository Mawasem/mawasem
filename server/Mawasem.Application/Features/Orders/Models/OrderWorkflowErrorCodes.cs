namespace Mawasem.Application.Features.Orders.Models;

public static class OrderWorkflowErrorCodes
{
    public const string OrderNotFound =
        "orders.order_not_found";

    public const string OrderAccessDenied =
        "orders.order_access_denied";

    public const string InvalidStatusTransition =
        "orders.invalid_status_transition";

    public const string InvalidReason =
        "orders.invalid_reason";

    public const string ConcurrencyConflict =
        "orders.concurrency_conflict";

    public const string OperationFailed =
        "orders.operation_failed";
}