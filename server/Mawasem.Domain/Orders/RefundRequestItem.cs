using Mawasem.Domain.Common;

namespace Mawasem.Domain.Orders;

public class RefundRequestItem : BaseAuditableEntity
{
    public int RefundRequestId { get; set; }

    public RefundRequest RefundRequest { get; set; } =
        null!;

    public int OrderItemId { get; set; }

    public OrderItem OrderItem { get; set; } =
        null!;

    // Quantity approved for refund.
    public int Quantity { get; set; }

    // Quantity physically received from the customer.
    public int ReturnedQuantity { get; set; }

    // Returned quantity that can be sold again.
    public int RestockQuantity { get; set; }

    public string? Reason { get; set; }

    public decimal UnitRefundAmount { get; set; }

    public decimal TotalRefundAmount { get; set; }
}