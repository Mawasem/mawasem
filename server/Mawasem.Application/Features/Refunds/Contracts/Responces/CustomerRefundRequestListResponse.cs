namespace Mawasem.Application.Features.Refunds.Contracts.Responses;

public sealed record CustomerRefundRequestListResponse
{
    public IReadOnlyCollection<
        CustomerRefundRequestListItemResponse> Items
    {
        get;
        init;
    } = Array.Empty<CustomerRefundRequestListItemResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}