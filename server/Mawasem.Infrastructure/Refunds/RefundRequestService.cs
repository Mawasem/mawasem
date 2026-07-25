using Mawasem.Application.Features.Refunds.Interfaces;
using Mawasem.Infrastructure.Persistence.Contexts;

namespace Mawasem.Infrastructure.Refunds;

public sealed partial class RefundRequestService
    : IRefundRequestService
{
    private const int MaximumIdempotencyKeyLength = 100;

    private const int MaximumCustomerReasonLength = 1000;

    private const int MaximumItemReasonLength = 1000;

    private readonly MawasemDbContext _dbContext;

    private readonly TimeProvider _timeProvider;

    public RefundRequestService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    private sealed record NormalizedRefundItemInput(
        int OrderItemId ,
        int Quantity ,
        string? Reason );
}