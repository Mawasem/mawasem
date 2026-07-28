namespace Mawasem.Application.Features.StoreReturns.Models;

public static class StoreReturnErrorCodes
{
    public const string InvalidRequest = "store_returns.invalid_request";
    public const string InvalidEmployee = "store_returns.invalid_employee";
    public const string OrderNotFound = "store_returns.order_not_found";
    public const string InvalidStoreOrder = "store_returns.invalid_store_order";
    public const string ItemsRequired = "store_returns.items_required";
    public const string DuplicateOrderItem = "store_returns.duplicate_order_item";
    public const string InvalidQuantity = "store_returns.invalid_quantity";
    public const string OrderItemNotFound = "store_returns.order_item_not_found";
    public const string QuantityExceedsReturnable = "store_returns.quantity_exceeds_returnable";
    public const string InvalidPaymentMethod = "store_returns.invalid_payment_method";
    public const string PaymentReferenceRequired = "store_returns.payment_reference_required";
    public const string ConcurrencyConflict = "store_returns.concurrency_conflict";
    public const string OperationFailed = "store_returns.operation_failed";
}