namespace Mawasem.Application.Features.StoreReturns.Contracts.Requests;

public sealed record CreateStoreReturnItemRequest
{
    public int OrderItemId { get; init; }

    public int Quantity { get; init; }

    public string? Reason { get; init; }
}