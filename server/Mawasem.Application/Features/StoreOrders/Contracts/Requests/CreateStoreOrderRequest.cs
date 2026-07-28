using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.StoreOrders.Contracts.Requests;

public sealed record CreateStoreOrderRequest
{
    public PaymentMethod PaymentMethod { get; init; }

    public string? PaymentReference { get; init; }

    public string? Notes { get; init; }

    public string IdempotencyKey { get; init; } =
        string.Empty;

    public IReadOnlyCollection<CreateStoreOrderItemRequest> Items
    {
        get;
        init;
    } = Array.Empty<CreateStoreOrderItemRequest>();
}