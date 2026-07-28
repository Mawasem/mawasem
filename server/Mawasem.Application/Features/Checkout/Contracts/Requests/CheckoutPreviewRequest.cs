using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Checkout.Contracts.Requests;

public sealed record CheckoutPreviewRequest
{
    // Required for HomeDelivery and ignored for StorePickup.
    public int? UserAddressId { get; init; }

    public DeliveryMethod DeliveryMethod { get; init; } =
        DeliveryMethod.HomeDelivery;

    public PaymentMethod PaymentMethod { get; init; } =
        PaymentMethod.CashOnDelivery;
}