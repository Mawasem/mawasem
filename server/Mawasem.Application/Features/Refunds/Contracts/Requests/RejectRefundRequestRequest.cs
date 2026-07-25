namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record RejectRefundRequestRequest
{
    public string AdminNotes { get; init; } =
        string.Empty;
}