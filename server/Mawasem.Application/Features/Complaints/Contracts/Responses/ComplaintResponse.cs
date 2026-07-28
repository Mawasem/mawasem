namespace Mawasem.Application.Features.Complaints.Contracts.Responses;

public sealed record ComplaintResponse
{
    public int Id { get; init; }

    public string CustomerName { get; init; } =
        string.Empty;

    public string CustomerPhone { get; init; } =
        string.Empty;

    public string ComplaintText { get; init; } =
        string.Empty;

    public int CreatedByEmployeeId { get; init; }

    public string CreatedByEmployeeNameAr { get; init; } =
        string.Empty;

    public string CreatedByEmployeeNameEn { get; init; } =
        string.Empty;

    public DateTimeOffset CreatedOn { get; init; }
}
