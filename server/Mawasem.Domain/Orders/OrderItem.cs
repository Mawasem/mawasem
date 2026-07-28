using Mawasem.Domain.Common;

namespace Mawasem.Domain.Orders;

public class OrderItem : BaseAuditableEntity
{
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    // Source product and variant
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int ProductVariantId { get; set; }

    public ProductVariant ProductVariant { get; set; } = null!;

    // Immutable product snapshot
    public string ProductNameAr { get; set; } = string.Empty;

    public string ProductNameEn { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string VariantSummaryAr { get; set; } = string.Empty;

    public string VariantSummaryEn { get; set; } = string.Empty;

    // Pricing
    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public int Quantity { get; set; }

    // Existing name retained to avoid an unnecessary column rename.
    // This represents the immutable line total.
    public decimal TotalPrice { get; set; }

    // Refund support
    public int RefundedQuantity { get; set; }

    public ICollection<StoreReturnItem> StoreReturnItems { get; set; } =
    new List<StoreReturnItem>();
}