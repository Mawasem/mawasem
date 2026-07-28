namespace Mawasem.Application.Features.StoreOrders.Models;

public static class StorePickupCollectionErrorCodes
{
    public const string InvalidRequest =
        "store_pickup.invalid_request";

    public const string InvalidEmployee =
        "store_pickup.invalid_employee";

    public const string EmployeeNotFound =
        "store_pickup.employee_not_found";

    public const string EmployeeBlocked =
        "store_pickup.employee_blocked";

    public const string OrderNotFound =
        "store_pickup.order_not_found";

    public const string NotStorePickupOrder =
        "store_pickup.not_store_pickup_order";

    public const string InvalidOrderStatus =
        "store_pickup.invalid_order_status";

    public const string AlreadyCollected =
        "store_pickup.already_collected";

    public const string InvalidPaymentMethod =
        "store_pickup.invalid_payment_method";

    public const string PaymentReferenceRequired =
        "store_pickup.payment_reference_required";

    public const string InvalidPaymentReference =
        "store_pickup.invalid_payment_reference";

    public const string InvalidNotes =
        "store_pickup.invalid_notes";

    public const string ConcurrencyConflict =
        "store_pickup.concurrency_conflict";

    public const string OperationFailed =
        "store_pickup.operation_failed";
}