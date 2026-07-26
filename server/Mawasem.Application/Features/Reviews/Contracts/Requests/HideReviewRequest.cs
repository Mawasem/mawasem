namespace Mawasem.Application.Features.Reviews.Contracts.Requests;

public sealed record HideReviewRequest
{
    public string ModerationReason { get; init; } =
        string.Empty;
}