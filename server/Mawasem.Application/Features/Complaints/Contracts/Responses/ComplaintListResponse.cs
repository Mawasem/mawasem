namespace Mawasem.Application.Features.Complaints.Contracts.Responses;

public sealed record ComplaintListResponse
{
    public IReadOnlyCollection<ComplaintResponse> Items { get; init; } =
        Array.Empty<ComplaintResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}
