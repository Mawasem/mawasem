namespace Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;

public sealed record DeliveryAreaListResponse
{
    public IReadOnlyCollection<DeliveryAreaResponse> Items { get; init; } =
        Array.Empty<DeliveryAreaResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}