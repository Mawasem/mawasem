using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record CustomerOrderListItemResponse
{
    public int Id { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public DateTime OrderDate { get; init; }

    public OrderStatus OrderStatus { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public PaymentStatus PaymentStatus { get; init; }

    public DeliveryMethod DeliveryMethod { get; init; }

    public decimal SubTotal { get; init; }

    public decimal Discount { get; init; }

    public decimal DeliveryFee { get; init; }

    public decimal TotalAmount { get; init; }

    public int DistinctItemCount { get; init; }

    public int TotalQuantity { get; init; }

    public bool CanCancel { get; init; }
}