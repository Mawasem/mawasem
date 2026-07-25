namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record ApproveRefundRequestRequest
{
    public string? AdminNotes { get; init; }
}