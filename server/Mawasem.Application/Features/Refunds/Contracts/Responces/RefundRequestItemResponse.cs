namespace Mawasem.Application.Features.Refunds.Contracts.Responses;

public sealed record RefundRequestItemResponse
{
    public int Id { get; init; }

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

    // Quantity approved for refund.
    public int Quantity { get; init; }

    // Quantity physically received from the customer.
    public int ReturnedQuantity { get; init; }

    // Returned quantity restored to sellable stock.
    public int RestockQuantity { get; init; }

    public string? Reason { get; init; }

    public decimal UnitRefundAmount { get; init; }

    public decimal TotalRefundAmount { get; init; }
}