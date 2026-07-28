using Mawasem.Application.Features.Complaints.Interfaces;
using Mawasem.Infrastructure.Persistence.Contexts;

namespace Mawasem.Infrastructure.Complaints;

public sealed partial class ComplaintManagementService
    : IComplaintManagementService
{
    private const int MaximumPageSize = 100;

    private const int MaximumSearchLength = 256;

    private const int MaximumCustomerNameLength = 200;

    private const int MaximumCustomerPhoneLength = 30;

    private const int MaximumComplaintTextLength = 2000;

    private readonly MawasemDbContext _dbContext;

    private readonly TimeProvider _timeProvider;

    public ComplaintManagementService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }
}
