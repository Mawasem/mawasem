using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Orders.Contracts.Requests;

public sealed record GetAdminOrdersRequest
{
    public string? Search { get; init; }

    public int? CustomerUserId { get; init; }

    public OrderStatus? Status { get; init; }

    public PaymentMethod? PaymentMethod { get; init; }

    public PaymentStatus? PaymentStatus { get; init; }

    public DeliveryMethod? DeliveryMethod { get; init; }

    public OrderSource? OrderSource { get; init; }

    public int? DeliveryAreaId { get; init; }

    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}