namespace Mawasem.Application.Features.StoreOrders.Contracts.Responses;

public sealed record StoreOrderReceiptItemResponse
{
    public int OrderItemId { get; init; }

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

    public decimal DiscountAmount { get; init; }

    public int Quantity { get; init; }

    public decimal LineTotal { get; init; }
}