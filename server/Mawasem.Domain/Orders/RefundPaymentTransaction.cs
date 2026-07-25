using Mawasem.Domain.Common;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;

namespace Mawasem.Domain.Orders;

public class RefundPaymentTransaction
    : BaseAuditableEntity
{
    public int RefundRequestId { get; set; }

    public RefundRequest RefundRequest { get; set; } =
        null!;

    // None represents a manually confirmed refund,
    // such as a cash refund.
    public PaymentGateway PaymentGateway { get; set; } =
        PaymentGateway.None;

    public RefundPaymentStatus Status { get; set; } =
        RefundPaymentStatus.Pending;

    // Product refund amount only.
    // Delivery fees are never included.
    public decimal Amount { get; set; }

    // Identifies one logical payment-refund attempt and
    // prevents accidental duplicate provider requests.
    public string IdempotencyKey { get; set; } =
        string.Empty;

    // Identifier returned by the payment gateway.
    public string? ProviderTransactionId { get; set; }

    // Additional provider reference, merchant reference,
    // or external correlation value.
    public string? ProviderReference { get; set; }

    public string? FailureCode { get; set; }

    public string? FailureMessage { get; set; }

    public DateTime RequestedAt { get; set; } =
        DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    // The employee who initiated the refund.
    // This may be null for a future system-initiated retry.
    public int? InitiatedByEmployeeId { get; set; }

    public ApplicationUser? InitiatedByEmployee
    {
        get;
        set;
    }

    // For manual refunds this records the employee who
    // confirmed that the customer received the money.
    // It remains null when completion is confirmed by a
    // payment-gateway callback.
    public int? CompletedByEmployeeId { get; set; }

    public ApplicationUser? CompletedByEmployee
    {
        get;
        set;
    }
}