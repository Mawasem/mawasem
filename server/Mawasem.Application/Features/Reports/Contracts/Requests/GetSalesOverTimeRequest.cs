using Mawasem.Application.Features.Reports.Models;

namespace Mawasem.Application.Features.Reports.Contracts.Requests;

public sealed record GetSalesOverTimeRequest
{
    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public SalesReportGranularity Granularity
    {
        get;
        init;
    } = SalesReportGranularity.Day;
}
