using Mawasem.Application.Features.Checkout.Interfaces;
using Mawasem.Domain.Carts;
using Mawasem.Domain.Delivery;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;
using Mawasem.Infrastructure.Persistence.Contexts;

namespace Mawasem.Infrastructure.Checkout;

public sealed partial class CheckoutService : ICheckoutService
{
    private const int MaxIdempotencyKeyLength = 128;

    private const int MaxNotesLength = 1000;

    private readonly MawasemDbContext _dbContext;

    private readonly TimeProvider _timeProvider;

    public CheckoutService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    private sealed record CheckoutContext
    {
        public required ApplicationUser Customer { get; init; }

        public required Cart Cart { get; init; }

        public required UserAddress? Address { get; init; }

        public required DeliveryMethod DeliveryMethod { get; init; }

        public required IReadOnlyCollection<CheckoutLine> Lines { get; init; }

        public required decimal SubTotal { get; init; }

        public required decimal Discount { get; init; }

        public required decimal DeliveryFee { get; init; }

        public required decimal TotalAmount { get; init; }
    }

    private sealed record CheckoutLine
    {
        public required CartItem CartItem { get; init; }

        public required Product Product { get; init; }

        public required ProductVariant Variant { get; init; }

        public required string VariantSummaryAr { get; init; }

        public required string VariantSummaryEn { get; init; }

        public required decimal UnitPrice { get; init; }

        public required decimal LineTotal { get; init; }
    }
}
