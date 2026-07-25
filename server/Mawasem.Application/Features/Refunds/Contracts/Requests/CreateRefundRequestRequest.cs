namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record CreateRefundRequestRequest
{
    public string IdempotencyKey { get; init; } =
        string.Empty;

    public string CustomerReason { get; init; } =
        string.Empty;

    public IReadOnlyCollection<CreateRefundRequestItemRequest> Items
    {
        get;
        init;
    } = Array.Empty<CreateRefundRequestItemRequest>();
}