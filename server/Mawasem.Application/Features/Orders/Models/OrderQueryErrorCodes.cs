namespace Mawasem.Application.Features.Orders.Models;

public static class OrderQueryErrorCodes
{
    public const string InvalidRequest =
        "orders.invalid_request";

    public const string CustomerNotFound =
        "orders.customer_not_found";

    public const string CustomerBlocked =
        "orders.customer_blocked";

    public const string OrderNotFound =
        "orders.order_not_found";

    public const string OrderAccessDenied =
        "orders.order_access_denied";

    public const string OperationFailed =
        "orders.query_failed";
}