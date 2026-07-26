using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Reports.Contracts.Requests;

public sealed record GetEmployeeReportRequest
{
    public string? Search { get; init; }

    public int? EmployeeId { get; init; }

    public string? Role { get; init; }

    public OrderStatus? ActionStatus { get; init; }

    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
