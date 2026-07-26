namespace Mawasem.Application.Features.Reviews.Contracts.Requests;

public sealed record CreateReviewRequest
{
    public int Rating { get; init; }

    public string Comment { get; init; } =
        string.Empty;
}