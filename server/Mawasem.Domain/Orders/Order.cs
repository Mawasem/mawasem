using Mawasem.Domain.Common;
using Mawasem.Domain.Delivery;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;

namespace Mawasem.Domain.Orders;

public class Order : BaseAuditableEntity
{
    // Customer
    public int UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    // Immutable customer snapshot
    public string CustomerNameAr { get; set; } = string.Empty;

    public string CustomerNameEn { get; set; } = string.Empty;

    public string CustomerPhone { get; set; } = string.Empty;

    // Source customer address
    public int? UserAddressId { get; set; }

    public UserAddress? UserAddress { get; set; }

    // Source delivery area
    public int? ShippingDeliveryAreaId { get; set; }

    public DeliveryArea? ShippingDeliveryArea { get; set; }

    // Immutable shipping snapshot
    // These fields remain null for future store-pickup orders.
    public string? ShippingRecipientName { get; set; }

    public string? ShippingRecipientPhone { get; set; }

    public string? ShippingCity { get; set; }

    public string? ShippingAreaName { get; set; }

    public string? ShippingDetailedAddress { get; set; }

    public string? ShippingBuildingNumber { get; set; }

    public string? ShippingFloorNumber { get; set; }

    public string? ShippingApartmentNumber { get; set; }

    public string? ShippingLandmark { get; set; }

    public string? ShippingDeliveryAreaNameAr { get; set; }

    public string? ShippingDeliveryAreaNameEn { get; set; }

    // Order information
    public string OrderNumber { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string? IdempotencyKey { get; set; }

    // Financial
    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal DeliveryFee { get; set; }

    public decimal TotalAmount { get; set; }

    public string? CouponCode { get; set; }

    // Status and payment
    public OrderStatus OrderStatus { get; set; } =
        OrderStatus.Pending;

    public PaymentMethod PaymentMethod { get; set; } =
        PaymentMethod.CashOnDelivery;

    public PaymentStatus PaymentStatus { get; set; } =
        PaymentStatus.Pending;

    public DeliveryMethod DeliveryMethod { get; set; } =
        DeliveryMethod.HomeDelivery;

    public OrderSource OrderSource { get; set; } =
        OrderSource.Website;

    // Notes and workflow metadata
    public string? Notes { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime? CancelledAtUtc { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime? RejectedAtUtc { get; set; }

    public DateTime? StockRestoredAtUtc { get; set; }

    // Navigation properties
    public ICollection<OrderItem> OrderItems { get; set; } =
        new List<OrderItem>();

    public ICollection<OrderStatusHistory> StatusHistory { get; set; } =
        new List<OrderStatusHistory>();
    public ICollection<RefundRequest> RefundRequests { get; set; } =
        new List<RefundRequest>();
}
