namespace Mawasem.Application.Features.Complaints.Contracts.Requests;

public sealed record GetComplaintsRequest
{
    public string? Search { get; init; }

    public int? CreatedByEmployeeId { get; init; }

    public DateTimeOffset? FromDateUtc { get; init; }

    public DateTimeOffset? ToDateUtc { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
