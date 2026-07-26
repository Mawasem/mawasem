using Mawasem.Application.Features.Reports.Interfaces;
using Mawasem.Domain.Identity;
using Mawasem.Infrastructure.Persistence.Contexts;

namespace Mawasem.Infrastructure.Reports;

public sealed partial class ReportService
    : IReportService
{
    private const int MaximumPageSize = 100;

    private const int MaximumSearchLength = 256;

    private readonly MawasemDbContext _dbContext;

    public ReportService(
        MawasemDbContext dbContext )
    {
        _dbContext =
            dbContext
            ?? throw new ArgumentNullException(
                nameof(dbContext));
    }

    private static bool TryCalculateSkipCount(
        int pageNumber ,
        int pageSize ,
        out int skipCount ,
        out string errorMessage )
    {
        skipCount = 0;
        errorMessage = string.Empty;

        if ( pageNumber <= 0 )
        {
            errorMessage =
                "Page number must be greater than zero.";

            return false;
        }

        if ( pageSize <= 0 ||
            pageSize > MaximumPageSize )
        {
            errorMessage =
                $"Page size must be between 1 and " +
                $"{MaximumPageSize}.";

            return false;
        }

        var calculatedSkipCount =
            (long)( pageNumber - 1 ) *
            pageSize;

        if ( calculatedSkipCount > int.MaxValue )
        {
            errorMessage =
                "The requested page is outside the supported range.";

            return false;
        }

        skipCount =
            (int)calculatedSkipCount;

        return true;
    }

    private static string? NormalizeSearch(
        string? search )
    {
        return string.IsNullOrWhiteSpace(search)
            ? null
            : search.Trim();
    }

    private static string? ResolveDashboardRole(
        string? requestedRole )
    {
        if ( string.IsNullOrWhiteSpace(requestedRole) )
        {
            return null;
        }

        var normalizedRole =
            requestedRole.Trim();

        return SystemRoles.DashboardRoles
            .FirstOrDefault(roleName =>
                string.Equals(
                    roleName ,
                    normalizedRole ,
                    StringComparison.OrdinalIgnoreCase));
    }

    private static int CalculateTotalPages(
        int totalCount ,
        int pageSize )
    {
        return totalCount == 0
            ? 0
            : (int)Math.Ceiling(
                totalCount /
                (double)pageSize);
    }
}
