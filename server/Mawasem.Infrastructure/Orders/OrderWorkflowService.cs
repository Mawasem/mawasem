using Mawasem.Application.Features.Orders.Interfaces;
using Mawasem.Infrastructure.Persistence.Contexts;

namespace Mawasem.Infrastructure.Orders;

public sealed partial class OrderWorkflowService
    : IOrderWorkflowService
{
    private const int MaxReasonLength = 500;

    private readonly MawasemDbContext _dbContext;

    private readonly TimeProvider _timeProvider;

    public OrderWorkflowService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }
}