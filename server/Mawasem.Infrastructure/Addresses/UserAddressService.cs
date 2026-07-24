using Mawasem.Application.Features.Addresses.Interfaces;
using Mawasem.Infrastructure.Persistence.Contexts;

namespace Mawasem.Infrastructure.Addresses;

public sealed partial class UserAddressService : IUserAddressService
{
    private const int MaximumLabelLength = 100;
    private const int MaximumCityLength = 100;
    private const int MaximumAreaNameLength = 200;
    private const int MaximumDetailedAddressLength = 500;
    private const int MaximumAddressPartLength = 50;
    private const int MaximumLandmarkLength = 300;
    private const int MaximumRecipientNameLength = 200;
    private const int MaximumDeliveryAreaNameLength = 200;

    private readonly MawasemDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public UserAddressService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }
}