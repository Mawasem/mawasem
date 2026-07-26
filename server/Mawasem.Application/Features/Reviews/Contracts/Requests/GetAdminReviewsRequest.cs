namespace Mawasem.Application.Features.Reviews.Contracts.Requests;

public sealed record GetAdminReviewsRequest
{
    public string? Search { get; init; }

    public int? ProductId { get; init; }

    public int? CustomerUserId { get; init; }

    public bool? IsVisible { get; init; }

    public DateTimeOffset? FromDateUtc { get; init; }

    public DateTimeOffset? ToDateUtc { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}