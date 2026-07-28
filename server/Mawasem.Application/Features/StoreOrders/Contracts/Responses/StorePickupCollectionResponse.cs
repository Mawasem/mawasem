using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.StoreOrders.Contracts.Responses;

public sealed record StorePickupCollectionResponse
{
    public int OrderId { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public OrderStatus OrderStatus { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public PaymentStatus PaymentStatus { get; init; }

    public string? PaymentReference { get; init; }

    public DateTime? PaidAtUtc { get; init; }

    public decimal TotalAmount { get; init; }

    public int CollectedByEmployeeId { get; init; }
}