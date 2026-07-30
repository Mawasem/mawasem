using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Checkout.Contracts.Requests;

public sealed record CheckoutPreviewRequest
{
    public int? UserAddressId { get; init; }

    public DeliveryMethod DeliveryMethod { get; init; } =
        DeliveryMethod.HomeDelivery;

    public PaymentMethod PaymentMethod { get; init; } =
        PaymentMethod.CashOnDelivery;
}
