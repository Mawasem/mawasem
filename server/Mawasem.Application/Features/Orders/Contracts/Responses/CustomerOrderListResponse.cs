namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record CustomerOrderListResponse
{
    public IReadOnlyCollection<CustomerOrderListItemResponse> Items
    {
        get;
        init;
    } = Array.Empty<CustomerOrderListItemResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}