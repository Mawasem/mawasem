namespace Mawasem.Application.Features.Reports.Contracts.Requests;

public sealed record GetBusinessDashboardRequest
{
    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }
}
