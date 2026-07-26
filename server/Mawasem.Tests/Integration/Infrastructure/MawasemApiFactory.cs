using Mawasem.API.BackgroundServices;
using Mawasem.Application.Features.Authentication.Options;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Mawasem.Tests.Integration.Infrastructure;

public sealed class MawasemApiFactory
    : WebApplicationFactory<Program>
{
    public const string SuperAdminEmail =
        "superadmin.integration@mawasem.test";

    public const string SuperAdminPassword =
        "Integration1!";

    private const string TestJwtIssuer =
        "Mawasem.API.IntegrationTests";

    private const string TestJwtAudience =
        "Mawasem.Tests";

    private const string TestJwtKey =
        "MDEyMzQ1Njc4OUFCQ0RFRjAxMjM0NTY3ODlBQkNERUY=";

    private const int TestAccessTokenMinutes =
        30;

    private const int TestRefreshTokenDays =
        1;

    private readonly string _databasePath =
        Path.Combine(
            Path.GetTempPath() ,
            $"mawasem-api-integration-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(
        IWebHostBuilder builder )
    {
        builder.UseEnvironment(
            "Development");

        builder.ConfigureAppConfiguration(
            ( _ , configurationBuilder ) =>
            {
                configurationBuilder.AddInMemoryCollection(
                    new Dictionary<string , string?>
                    {
                        ["Jwt:Issuer"] =
                            TestJwtIssuer ,

                        ["Jwt:Audience"] =
                            TestJwtAudience ,

                        ["Jwt:Key"] =
                            TestJwtKey ,

                        ["Jwt:AccessTokenMinutes"] =
                            TestAccessTokenMinutes.ToString() ,

                        ["Jwt:RefreshTokenDays"] =
                            TestRefreshTokenDays.ToString() ,

                        ["AdminSeed:Email"] =
                            SuperAdminEmail ,

                        ["AdminSeed:Password"] =
                            SuperAdminPassword ,

                        ["AdminSeed:FullNameAr"] =
                            "مدير النظام للاختبارات" ,

                        ["AdminSeed:FullNameEn"] =
                            "Integration Test Super Admin" ,

                        ["Logging:LogLevel:Default"] =
                            "Warning" ,

                        ["Logging:LogLevel:Microsoft.EntityFrameworkCore.Database.Command"] =
                            "Warning"
                    });
            });

        builder.ConfigureServices(
            services =>
            {
                RemovePendingImageDeletionWorker(
                    services);

                RemoveProductionDatabase(
                    services);

                ConfigureTestJwt(
                    services);

                services.AddDbContext<MawasemDbContext>(
                    options =>
                    {
                        options.UseSqlite(
                            $"Data Source={_databasePath};Foreign Keys=True");
                    });

                using var serviceProvider =
                    services.BuildServiceProvider();

                using var scope =
                    serviceProvider.CreateScope();

                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<MawasemDbContext>();

                dbContext.Database.EnsureDeleted();

                dbContext.Database.EnsureCreated();
            });
    }

    protected override void Dispose(
        bool disposing )
    {
        base.Dispose(disposing);

        if ( !disposing )
        {
            return;
        }

        SqliteConnection.ClearAllPools();

        TryDeleteDatabase();
    }

    private static void ConfigureTestJwt(
        IServiceCollection services )
    {
        var testJwtSettings =
            new JwtSettings
            {
                Issuer =
                    TestJwtIssuer ,

                Audience =
                    TestJwtAudience ,

                Key =
                    TestJwtKey ,

                AccessTokenMinutes =
                    TestAccessTokenMinutes ,

                RefreshTokenDays =
                    TestRefreshTokenDays
            };

        services.AddSingleton<
            IOptions<JwtSettings>>(
            Options.Create(
                testJwtSettings));

        var signingKey =
            new SymmetricSecurityKey(
                Convert.FromBase64String(
                    TestJwtKey));

        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme ,
            options =>
            {
                options.TokenValidationParameters ??=
                    new TokenValidationParameters();

                options.TokenValidationParameters
                    .ValidateIssuer = true;

                options.TokenValidationParameters
                    .ValidIssuer = TestJwtIssuer;

                options.TokenValidationParameters
                    .ValidateAudience = true;

                options.TokenValidationParameters
                    .ValidAudience = TestJwtAudience;

                options.TokenValidationParameters
                    .ValidateIssuerSigningKey = true;

                options.TokenValidationParameters
                    .IssuerSigningKey = signingKey;

                options.TokenValidationParameters
                    .ValidateLifetime = true;

                options.TokenValidationParameters
                    .RequireExpirationTime = true;

                options.TokenValidationParameters
                    .ClockSkew =
                        TimeSpan.FromSeconds(30);
            });
    }

    private static void RemovePendingImageDeletionWorker(
        IServiceCollection services )
    {
        var descriptors =
            services
                .Where(descriptor =>
                    descriptor.ServiceType ==
                    typeof(IHostedService) &&
                    descriptor.ImplementationType ==
                    typeof(PendingProductImageDeletionWorker))
                .ToArray();

        foreach ( var descriptor in descriptors )
        {
            services.Remove(descriptor);
        }
    }

    private static void RemoveProductionDatabase(
        IServiceCollection services )
    {
        var descriptors =
            services
                .Where(descriptor =>
                    descriptor.ServiceType ==
                    typeof(DbContextOptions<MawasemDbContext>) ||
                    descriptor.ServiceType ==
                    typeof(MawasemDbContext))
                .ToArray();

        foreach ( var descriptor in descriptors )
        {
            services.Remove(descriptor);
        }
    }

    private void TryDeleteDatabase()
    {
        if ( !File.Exists(_databasePath) )
        {
            return;
        }

        try
        {
            File.Delete(_databasePath);
        }
        catch ( IOException )
        {
            // A pooled SQLite handle may remain briefly on Windows.
            // Cleanup must not change the HTTP test result.
        }
        catch ( UnauthorizedAccessException )
        {
            // Cleanup must not change the HTTP test result.
        }
    }
}