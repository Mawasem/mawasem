namespace Mawasem.Application.Features.Reviews.Contracts.Responses;

public sealed record PagedReviewResponse<TItem>
{
    public IReadOnlyCollection<TItem> Items { get; init; } =
        Array.Empty<TItem>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}