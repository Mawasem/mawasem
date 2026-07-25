namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record AdminOrderListResponse
{
    public IReadOnlyCollection<AdminOrderListItemResponse> Items
    {
        get;
        init;
    } = Array.Empty<AdminOrderListItemResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}