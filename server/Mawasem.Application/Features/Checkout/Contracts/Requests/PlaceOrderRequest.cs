using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Checkout.Contracts.Requests;

public sealed record PlaceOrderRequest
{
    public int UserAddressId { get; init; }

    public PaymentMethod PaymentMethod { get; init; } =
        PaymentMethod.CashOnDelivery;

    public string? Notes { get; init; }

    public string IdempotencyKey { get; init; } =
        string.Empty;
}