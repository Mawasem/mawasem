namespace Mawasem.Application.Features.Complaints.Contracts.Requests;

public sealed record CreateComplaintRequest
{
    public string CustomerName { get; init; } =
        string.Empty;

    public string CustomerPhone { get; init; } =
        string.Empty;

    public string ComplaintText { get; init; } =
        string.Empty;
}
