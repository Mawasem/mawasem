using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.StoreOrders.Contracts.Requests;

public sealed record CollectStorePickupOrderRequest
{
    public PaymentMethod PaymentMethod { get; init; } =
        PaymentMethod.CashAtStore;

    public string? PaymentReference { get; init; }

    public string? Notes { get; init; }
}