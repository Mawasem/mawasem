namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record SalesOverTimePointResponse
{
    public DateTime PeriodStartUtc { get; init; }

    public int DeliveredOrders { get; init; }

    public decimal GrossSales { get; init; }

    public decimal CompletedRefundAmount { get; init; }

    public decimal NetRevenue { get; init; }
}
