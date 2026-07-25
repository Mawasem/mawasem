namespace Mawasem.Application.Features.Refunds.Contracts.Requests;

public sealed record CompleteRefundRequestRequest
{
    // Identifies one logical money-refund confirmation.
    // Retrying with the same key must not create another
    // payment transaction or restore stock twice.
    public string PaymentIdempotencyKey { get; init; } =
        string.Empty;

    // Required when the original order was paid online.
    // For Paymob, this stores the confirmed provider-side
    // refund transaction identifier.
    public string? ProviderTransactionId { get; init; }

    // Optional merchant reference, receipt number,
    // settlement reference, or other external correlation.
    public string? ProviderReference { get; init; }

    public IReadOnlyCollection<
        CompleteRefundRequestItemRequest> Items
    {
        get;
        init;
    } = Array.Empty<CompleteRefundRequestItemRequest>();
}