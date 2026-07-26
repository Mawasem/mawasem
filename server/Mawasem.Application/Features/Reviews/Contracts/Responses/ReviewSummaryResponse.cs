namespace Mawasem.Application.Features.Reviews.Contracts.Responses;

public sealed record ReviewSummaryResponse
{
    public decimal AverageRating { get; init; }

    public int TotalCount { get; init; }

    public IReadOnlyCollection<ReviewRatingCountResponse>
        Distribution
    {
        get;
        init;
    } = Array.Empty<ReviewRatingCountResponse>();
}