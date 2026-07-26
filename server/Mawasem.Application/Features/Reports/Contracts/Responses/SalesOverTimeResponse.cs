using Mawasem.Application.Features.Reports.Models;

namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record SalesOverTimeResponse
{
    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public SalesReportGranularity Granularity
    {
        get;
        init;
    }

    public int TotalDeliveredOrders { get; init; }

    public decimal TotalGrossSales { get; init; }

    public decimal TotalCompletedRefundAmount { get; init; }

    public decimal TotalNetRevenue { get; init; }

    public IReadOnlyList<SalesOverTimePointResponse>
        Items { get; init; } =
            Array.Empty<SalesOverTimePointResponse>();
}
