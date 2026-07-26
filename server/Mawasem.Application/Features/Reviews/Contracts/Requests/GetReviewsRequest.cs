namespace Mawasem.Application.Features.Reviews.Contracts.Requests;

public sealed record GetReviewsRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}