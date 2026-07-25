using Mawasem.Domain.Common;
using Mawasem.Domain.Enums;

namespace Mawasem.Domain.Orders;

public class RefundRequest : BaseAuditableEntity
{
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    // Prevents duplicate requests when the customer
    // retries the same operation.
    public string IdempotencyKey { get; set; } =
        string.Empty;

    public RefundStatus Status { get; set; } =
        RefundStatus.Pending;

    public string CustomerReason { get; set; } =
        string.Empty;

    public string? AdminNotes { get; set; }

    // Immutable total calculated from the order-item
    // price snapshots when the request is created.
    public decimal RefundAmount { get; set; }

    // All workflow timestamps are stored in UTC.
    public DateTime RequestedAt { get; set; } =
        DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    public int? ReviewedByEmployeeId { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int? CompletedByEmployeeId { get; set; }

    // Prevents returned stock from being restored twice.
    public DateTime? StockRestoredAtUtc { get; set; }

    public ICollection<RefundRequestItem> Items
    {
        get;
        set;
    } = new List<RefundRequestItem>();

    public ICollection<RefundPaymentTransaction>
        PaymentTransactions
    {
        get;
        set;
    } = new List<RefundPaymentTransaction>();
}