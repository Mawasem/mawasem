using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record OrderWorkflowResponse
{
    public int OrderId { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public OrderStatus PreviousStatus { get; init; }

    public OrderStatus CurrentStatus { get; init; }

    public bool StatusChanged { get; init; }

    public bool StockRestored { get; init; }

    public DateTime? StockRestoredAtUtc { get; init; }
}