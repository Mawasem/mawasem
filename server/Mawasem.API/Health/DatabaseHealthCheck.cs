using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Mawasem.API.Health;

public sealed class DatabaseHealthCheck
    : IHealthCheck
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseHealthCheck(
        IServiceScopeFactory scopeFactory )
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<HealthCheckResult>
        CheckHealthAsync(
            HealthCheckContext context ,
            CancellationToken cancellationToken = default )
    {
        try
        {
            await using var scope =
                _scopeFactory.CreateAsyncScope();

            var dbContext =
                scope.ServiceProvider
                    .GetRequiredService<MawasemDbContext>();

            var canConnect =
                await dbContext.Database.CanConnectAsync(
                    cancellationToken);

            return canConnect
                ? HealthCheckResult.Healthy(
                    "The database connection is available.")
                : HealthCheckResult.Unhealthy(
                    "The database connection is unavailable.");
        }
        catch ( Exception exception )
        {
            return HealthCheckResult.Unhealthy(
                "The database health check failed." ,
                exception);
        }
    }
}