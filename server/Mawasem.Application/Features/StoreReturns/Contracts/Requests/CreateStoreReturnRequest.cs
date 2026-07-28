using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.StoreReturns.Contracts.Requests;

public sealed record CreateStoreReturnRequest
{
    public PaymentMethod RefundPaymentMethod { get; init; }

    public string? RefundPaymentReference { get; init; }

    public IReadOnlyCollection<CreateStoreReturnItemRequest> Items
    {
        get;
        init;
    } = Array.Empty<CreateStoreReturnItemRequest>();
}