using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record AdminOrderListItemResponse
{
    public int Id { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public DateTime OrderDate { get; init; }

    // Null for anonymous walk-in store sales.
    public int? CustomerUserId { get; init; }

    public string CustomerNameAr { get; init; } =
        string.Empty;

    public string CustomerNameEn { get; init; } =
        string.Empty;

    public string CustomerPhone { get; init; } =
        string.Empty;

    public OrderStatus OrderStatus { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public PaymentStatus PaymentStatus { get; init; }

    public DeliveryMethod DeliveryMethod { get; init; }

    public OrderSource OrderSource { get; init; }

    public int? ShippingDeliveryAreaId { get; init; }

    public string? ShippingDeliveryAreaNameAr { get; init; }

    public string? ShippingDeliveryAreaNameEn { get; init; }

    public decimal SubTotal { get; init; }

    public decimal Discount { get; init; }

    public decimal DeliveryFee { get; init; }

    public decimal TotalAmount { get; init; }

    public int DistinctItemCount { get; init; }

    public int TotalQuantity { get; init; }

    public bool CanConfirm { get; init; }

    public bool CanReject { get; init; }

    public bool CanCancel { get; init; }
}