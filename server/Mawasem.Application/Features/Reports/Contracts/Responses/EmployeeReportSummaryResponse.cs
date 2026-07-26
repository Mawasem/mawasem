namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record EmployeeReportSummaryResponse
{
    public IReadOnlyCollection<EmployeeReportSummaryItemResponse>
        Items { get; init; } =
            Array.Empty<EmployeeReportSummaryItemResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}
