namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record CustomerOrderItemResponse
{
    public int Id { get; init; }

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

    public int RefundedQuantity { get; init; }
}