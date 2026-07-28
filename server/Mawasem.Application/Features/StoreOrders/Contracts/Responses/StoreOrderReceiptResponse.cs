using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.StoreOrders.Contracts.Responses;

public sealed record StoreOrderReceiptResponse
{
    public int OrderId { get; init; }

    public string ReceiptNumber { get; init; } =
        string.Empty;

    public DateTime OrderDate { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public PaymentStatus PaymentStatus { get; init; }

    public string? PaymentReference { get; init; }

    public DateTime? PaidAtUtc { get; init; }

    public decimal SubTotal { get; init; }

    public decimal Discount { get; init; }

    public decimal TotalAmount { get; init; }

    public int ProcessedByEmployeeId { get; init; }

    public IReadOnlyCollection<StoreOrderReceiptItemResponse> Items
    {
        get;
        init;
    } = Array.Empty<StoreOrderReceiptItemResponse>();
}