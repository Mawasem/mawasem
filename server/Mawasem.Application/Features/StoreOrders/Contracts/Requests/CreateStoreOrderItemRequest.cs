namespace Mawasem.Application.Features.StoreOrders.Contracts.Requests;

public sealed record CreateStoreOrderItemRequest
{
    public int ProductVariantId { get; init; }

    public int Quantity { get; init; }
}