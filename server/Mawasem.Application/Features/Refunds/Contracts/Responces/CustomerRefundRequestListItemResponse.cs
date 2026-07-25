using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Refunds.Contracts.Responses;

public sealed record CustomerRefundRequestListItemResponse
{
    public int Id { get; init; }

    public int OrderId { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public RefundStatus Status { get; init; }

    public string CustomerReason { get; init; } =
        string.Empty;

    public decimal RefundAmount { get; init; }

    public int ItemCount { get; init; }

    public int TotalQuantity { get; init; }

    public DateTime RequestedAt { get; init; }

    public DateTime? ReviewedAt { get; init; }

    public DateTime? CompletedAt { get; init; }
}