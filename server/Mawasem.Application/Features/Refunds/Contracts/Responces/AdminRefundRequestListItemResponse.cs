using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Refunds.Contracts.Responses;

public sealed record AdminRefundRequestListItemResponse
{
    public int Id { get; init; }

    public int OrderId { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public int? CustomerUserId { get; init; }

    public string CustomerNameAr { get; init; } =
        string.Empty;

    public string CustomerNameEn { get; init; } =
        string.Empty;

    public string CustomerPhone { get; init; } =
        string.Empty;

    public RefundStatus Status { get; init; }

    public string CustomerReason { get; init; } =
        string.Empty;

    public decimal RefundAmount { get; init; }

    public int ItemCount { get; init; }

    public int TotalQuantity { get; init; }

    public DateTime RequestedAt { get; init; }

    public DateTime? ReviewedAt { get; init; }

    public int? CompletedByEmployeeId { get; init; }

    public DateTime? CompletedAt { get; init; }

    public int? ReviewedByEmployeeId { get; init; }
}