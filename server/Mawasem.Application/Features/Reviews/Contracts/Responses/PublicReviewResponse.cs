namespace Mawasem.Application.Features.Reviews.Contracts.Responses;

public sealed record PublicReviewResponse
{
    public int Id { get; init; }

    public int ProductId { get; init; }

    public string CustomerDisplayName { get; init; } =
        string.Empty;

    public int Rating { get; init; }

    public string Comment { get; init; } =
        string.Empty;

    public DateTimeOffset CreatedOn { get; init; }

    public DateTimeOffset? LastModifiedOn { get; init; }
}