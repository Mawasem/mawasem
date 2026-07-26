namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record EmployeeOrderActionsResponse
{
    public int EmployeeId { get; init; }

    public string FullNameAr { get; init; } =
        string.Empty;

    public string FullNameEn { get; init; } =
        string.Empty;

    public string? Email { get; init; }

    public IReadOnlyCollection<string> Roles { get; init; } =
        Array.Empty<string>();

    public IReadOnlyCollection<EmployeeOrderActionResponse>
        Items { get; init; } =
            Array.Empty<EmployeeOrderActionResponse>();

    public int PageNumber { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages { get; init; }
}
