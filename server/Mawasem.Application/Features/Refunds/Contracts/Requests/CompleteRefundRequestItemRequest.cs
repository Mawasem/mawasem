namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record CompleteRefundRequestItemRequest
{
    public int RefundRequestItemId { get; init; }

    public int ReturnedQuantity { get; init; }

    public int RestockQuantity { get; init; }
}