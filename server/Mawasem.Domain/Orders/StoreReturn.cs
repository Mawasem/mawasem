using Mawasem.Domain.Common;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;

namespace Mawasem.Domain.Orders;

public class StoreReturn : BaseAuditableEntity
{
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public string ReturnNumber { get; set; } = string.Empty;

    public PaymentMethod RefundPaymentMethod { get; set; }

    public string? RefundPaymentReference { get; set; }

    public decimal TotalRefundAmount { get; set; }

    public DateTime ReturnedAtUtc { get; set; } = DateTime.UtcNow;

    public int ProcessedByEmployeeId { get; set; }

    public ApplicationUser ProcessedByEmployee { get; set; } = null!;

    public ICollection<StoreReturnItem> Items { get; set; } =
        new List<StoreReturnItem>();
}