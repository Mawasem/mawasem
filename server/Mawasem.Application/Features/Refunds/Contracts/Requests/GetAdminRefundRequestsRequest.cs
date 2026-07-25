using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record GetAdminRefundRequestsRequest
{
    public string? Search { get; init; }

    public RefundStatus? Status { get; init; }

    public int? CustomerUserId { get; init; }

    public int? OrderId { get; init; }

    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}