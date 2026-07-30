using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Checkout.Contracts.Responses;

public sealed record CheckoutPreviewResponse
{
    public int CartId { get; init; }

    public int UserAddressId { get; init; }

    public int DeliveryAreaId { get; init; }

    public IReadOnlyCollection<CheckoutItemResponse> Items { get; init; } =
        Array.Empty<CheckoutItemResponse>();

    public decimal SubTotal { get; init; }

    public decimal Discount { get; init; }

    public decimal DeliveryFee { get; init; }

    public decimal TotalAmount { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public bool CanPlaceOrder { get; init; }

    public IReadOnlyCollection<CheckoutWarningResponse> Warnings { get; init; } =
        Array.Empty<CheckoutWarningResponse>();
}

public sealed record CheckoutItemResponse
{
    public int CartItemId { get; init; }

    public int ProductId { get; init; }

    public int ProductVariantId { get; init; }

    public string ProductNameAr { get; init; } =
        string.Empty;

    public string ProductNameEn { get; init; } =
        string.Empty;

    public string Sku { get; init; } =
        string.Empty;

    public string VariantSummaryAr { get; init; } =
        string.Empty;

    public string VariantSummaryEn { get; init; } =
        string.Empty;

    public decimal UnitPrice { get; init; }

    public int Quantity { get; init; }

    public decimal LineTotal { get; init; }
}

public sealed record CheckoutWarningResponse
{
    public string Code { get; init; } =
        string.Empty;

    public string Message { get; init; } =
        string.Empty;
}