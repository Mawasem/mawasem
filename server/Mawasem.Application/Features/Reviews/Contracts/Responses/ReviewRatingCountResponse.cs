namespace Mawasem.Application.Features.Reviews.Contracts.Responses;

public sealed record ReviewRatingCountResponse
{
    public int Rating { get; init; }

    public int Count { get; init; }
}