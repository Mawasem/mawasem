namespace Mawasem.Application.Features.Reviews.Contracts.Responses;

public sealed record CustomerReviewResponse
{
    public int Id { get; init; }

    public int ProductId { get; init; }

    public string ProductNameAr { get; init; } =
        string.Empty;

    public string ProductNameEn { get; init; } =
        string.Empty;

    public int Rating { get; init; }

    public string Comment { get; init; } =
        string.Empty;

    public bool IsVisible { get; init; }

    public string? ModerationReason { get; init; }

    public DateTimeOffset? ModeratedAtUtc { get; init; }

    public DateTimeOffset CreatedOn { get; init; }

    public DateTimeOffset? LastModifiedOn { get; init; }
}