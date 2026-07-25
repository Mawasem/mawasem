using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record GetCustomerRefundRequestsRequest
{
    public string? Search { get; init; }

    public RefundStatus? Status { get; init; }

    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}