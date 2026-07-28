using Mawasem.Domain.Common;

namespace Mawasem.Domain.Orders;

public class StoreReturnItem : BaseAuditableEntity
{
    public int StoreReturnId { get; set; }

    public StoreReturn StoreReturn { get; set; } = null!;

    public int OrderItemId { get; set; }

    public OrderItem OrderItem { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitRefundAmount { get; set; }

    public decimal TotalRefundAmount { get; set; }

    public string? Reason { get; set; }
}