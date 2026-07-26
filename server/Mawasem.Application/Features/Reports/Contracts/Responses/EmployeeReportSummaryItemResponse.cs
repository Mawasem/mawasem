namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record EmployeeReportSummaryItemResponse
{
    public int EmployeeId { get; init; }

    public string FullNameAr { get; init; } =
        string.Empty;

    public string FullNameEn { get; init; } =
        string.Empty;

    public string? Email { get; init; }

    public bool IsBlocked { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } =
        Array.Empty<string>();

    public int TotalOrderActions { get; init; }

    public IReadOnlyCollection<EmployeeOrderActionCountResponse>
        OrderActions { get; init; } =
            Array.Empty<EmployeeOrderActionCountResponse>();
}
