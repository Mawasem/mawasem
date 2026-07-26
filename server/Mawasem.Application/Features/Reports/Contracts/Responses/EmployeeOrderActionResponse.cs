using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record EmployeeOrderActionResponse
{
    public int HistoryId { get; init; }

    public int OrderId { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public OrderStatus PreviousStatus { get; init; }

    public OrderStatus NewStatus { get; init; }

    public DateTime ChangedAtUtc { get; init; }

    public string? Reason { get; init; }

    public decimal TotalAmount { get; init; }

    public string CustomerNameAr { get; init; } =
        string.Empty;

    public string CustomerNameEn { get; init; } =
        string.Empty;
}
