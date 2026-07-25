namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record CreateRefundRequestItemRequest
{
    public int OrderItemId { get; init; }

    public int Quantity { get; init; }

    public string? Reason { get; init; }
}