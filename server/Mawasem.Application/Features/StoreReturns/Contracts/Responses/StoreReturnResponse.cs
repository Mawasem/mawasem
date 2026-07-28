using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.StoreReturns.Contracts.Responses;

public sealed record StoreReturnResponse
{
    public int StoreReturnId { get; init; }

    public string ReturnNumber { get; init; } = string.Empty;

    public int OrderId { get; init; }

    public string OrderNumber { get; init; } = string.Empty;

    public OrderStatus OrderStatus { get; init; }

    public decimal TotalRefundAmount { get; init; }

    public PaymentMethod RefundPaymentMethod { get; init; }

    public string? RefundPaymentReference { get; init; }

    public DateTime ReturnedAtUtc { get; init; }
}