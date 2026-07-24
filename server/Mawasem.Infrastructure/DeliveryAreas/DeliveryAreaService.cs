using Mawasem.Application.Features.DeliveryAreas.Interfaces;
using Mawasem.Infrastructure.Persistence.Contexts;

namespace Mawasem.Infrastructure.DeliveryAreas;

public sealed partial class DeliveryAreaService
    : IDeliveryAreaService
{
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = 200;
    private const int MaximumNameLength = 200;

    private readonly MawasemDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public DeliveryAreaService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }
}