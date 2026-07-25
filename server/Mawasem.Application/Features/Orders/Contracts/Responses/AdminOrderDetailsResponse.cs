using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record AdminOrderDetailsResponse
{
    public int Id { get; init; }

    public string OrderNumber { get; init; } =
        string.Empty;

    public DateTime OrderDate { get; init; }

    public OrderStatus OrderStatus { get; init; }

    public PaymentMethod PaymentMethod { get; init; }

    public PaymentStatus PaymentStatus { get; init; }

    public DeliveryMethod DeliveryMethod { get; init; }

    public OrderSource OrderSource { get; init; }

    public decimal SubTotal { get; init; }

    public decimal Discount { get; init; }

    public decimal DeliveryFee { get; init; }

    public decimal TotalAmount { get; init; }

    public string? CouponCode { get; init; }

    public string? Notes { get; init; }

    public string? IdempotencyKey { get; init; }

    public string? CancellationReason { get; init; }

    public DateTime? CancelledAtUtc { get; init; }

    public string? RejectionReason { get; init; }

    public DateTime? RejectedAtUtc { get; init; }

    public DateTime? StockRestoredAtUtc { get; init; }

    public int DistinctItemCount { get; init; }

    public int TotalQuantity { get; init; }

    public bool CanConfirm { get; init; }

    public bool CanReject { get; init; }

    public bool CanCancel { get; init; }

    public AdminOrderCustomerResponse Customer
    {
        get;
        init;
    } = new();

    public CustomerOrderShippingResponse Shipping
    {
        get;
        init;
    } = new();

    public IReadOnlyCollection<CustomerOrderItemResponse> Items
    {
        get;
        init;
    } = Array.Empty<CustomerOrderItemResponse>();
}