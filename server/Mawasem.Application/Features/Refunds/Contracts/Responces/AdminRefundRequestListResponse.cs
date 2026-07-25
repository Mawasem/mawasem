namespace Mawasem.Application.Features.Refunds.Contracts.Responses;

public sealed record AdminRefundRequestListResponse
{
    public IReadOnlyCollection<
        AdminRefundRequestListItemResponse> Items
    {
        get;
        init;
    } = Array.Empty<AdminRefundRequestListItemResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}