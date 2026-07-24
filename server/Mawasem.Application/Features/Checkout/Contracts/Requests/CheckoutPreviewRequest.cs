using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Checkout.Contracts.Requests;

public sealed record CheckoutPreviewRequest
{
    public int UserAddressId { get; init; }

    public PaymentMethod PaymentMethod { get; init; } =
        PaymentMethod.CashOnDelivery;
}