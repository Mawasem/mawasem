using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Refunds.Contracts.Responses;

public sealed record RefundRequestDetailsResponse
{
    public int Id { get; init; }

    public int OrderId { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public RefundStatus Status { get; init; }

    public string CustomerReason { get; init; } =
        string.Empty;

    public string? AdminNotes { get; init; }

    public decimal RefundAmount { get; init; }

    public DateTime RequestedAt { get; init; }

    public DateTime? ReviewedAt { get; init; }

    public int? ReviewedByEmployeeId { get; init; }

    public DateTime? CompletedAt { get; init; }

    public int? CompletedByEmployeeId { get; init; }

    public DateTime? StockRestoredAtUtc { get; init; }

    public IReadOnlyCollection<RefundRequestItemResponse> Items
    {
        get;
        init;
    } = Array.Empty<RefundRequestItemResponse>();
}