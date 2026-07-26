namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record BusinessDashboardResponse
{
    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public int TotalOrders { get; init; }

    public int DeliveredOrders { get; init; }

    public int PendingFulfillmentOrders { get; init; }

    public int CancelledOrders { get; init; }

    public int RejectedOrders { get; init; }

    public decimal GrossSales { get; init; }

    public decimal CompletedRefundAmount { get; init; }

    public decimal NetRevenue { get; init; }

    public decimal AverageOrderValue { get; init; }

    public IReadOnlyList<OrderStatusCountResponse>
        OrderStatusCounts { get; init; } =
            Array.Empty<OrderStatusCountResponse>();
}
