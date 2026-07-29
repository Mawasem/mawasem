using Mawasem.API.Authentication;
using Mawasem.API.Authorization;
using Mawasem.API.BackgroundServices;
using Mawasem.API.Configuration;
using Mawasem.API.Health;
using Mawasem.API.Middleware;
using Mawasem.Application.Features.Addresses.Interfaces;
using Mawasem.Application.Features.Authentication.Interfaces;
using Mawasem.Application.Features.Authentication.Models;
using Mawasem.Application.Features.Authentication.Options;
using Mawasem.Application.Features.Brands.Interfaces;
using Mawasem.Application.Features.Carts.Interfaces;
using Mawasem.Application.Features.Categories.Interfaces;
using Mawasem.Application.Features.Checkout.Interfaces;
using Mawasem.Application.Features.Collections.Interfaces;
using Mawasem.Application.Features.Complaints.Interfaces;
using Mawasem.Application.Features.Customers.Interfaces;
using Mawasem.Application.Features.DeliveryAreas.Interfaces;
using Mawasem.Application.Features.Employees.Interfaces;
using Mawasem.Application.Features.Orders.Interfaces;
using Mawasem.Application.Features.Products.Interfaces;
using Mawasem.Application.Features.PublicCatalog.Interfaces;
using Mawasem.Application.Features.Refunds.Interfaces;
using Mawasem.Application.Features.Reports.Interfaces;
using Mawasem.Application.Features.Reviews.Interfaces;
using Mawasem.Application.Features.Roles.Interfaces;
using Mawasem.Application.Features.Seasons.Interfaces;
using Mawasem.Application.Features.StoreOrders.Interfaces;
using Mawasem.Application.Features.StoreReturns.Interfaces;
using Mawasem.Domain.Identity;
using Mawasem.Infrastructure.Addresses;
using Mawasem.Infrastructure.Authentication;
using Mawasem.Infrastructure.Brands;
using Mawasem.Infrastructure.Carts;
using Mawasem.Infrastructure.Categories;
using Mawasem.Infrastructure.Checkout;
using Mawasem.Infrastructure.Collections;
using Mawasem.Infrastructure.Complaints;
using Mawasem.Infrastructure.Customers;
using Mawasem.Infrastructure.DeliveryAreas;
using Mawasem.Infrastructure.Employees;
using Mawasem.Infrastructure.Orders;
using Mawasem.Infrastructure.Persistence.Contexts;
using Mawasem.Infrastructure.Persistence.Seed;
using Mawasem.Infrastructure.Products;
using Mawasem.Infrastructure.PublicCatalog;
using Mawasem.Infrastructure.Refunds;
using Mawasem.Infrastructure.Reports;
using Mawasem.Infrastructure.Reviews;
using Mawasem.Infrastructure.Roles;
using Mawasem.Infrastructure.Seasons;
using Mawasem.Infrastructure.Storage.Images;
using Mawasem.Infrastructure.StoreOrders;
using Mawasem.Infrastructure.StoreReturns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Globalization;
using System.Threading.RateLimiting;

var builder =
    WebApplication.CreateBuilder(args);

var isDevelopment =
    builder.Environment.IsDevelopment();

if ( !isDevelopment )
{
    builder.Logging.ClearProviders();

    builder.Logging.AddJsonConsole(
        options =>
        {
            options.IncludeScopes = true;
            options.UseUtcTimestamp = true;
            options.TimestampFormat =
                "yyyy-MM-ddTHH:mm:ss.fffZ";
        });
}

builder.Services.AddControllers();

builder.Services.AddProblemDetails(
    options =>
    {
        options.CustomizeProblemDetails =
            context =>
            {
                context.ProblemDetails.Extensions["traceId"] =
                    Activity.Current?.Id ??
                    context.HttpContext.TraceIdentifier;

                if ( context.ProblemDetails.Status is >= 500 )
                {
                    context.ProblemDetails.Title =
                        "Unexpected server error.";

                    context.ProblemDetails.Detail =
                        "An unexpected error occurred while processing the request.";
                }
            };
    });

var frontendOptions =
    builder.Configuration
        .GetSection(FrontendOptions.SectionName)
        .Get<FrontendOptions>()
    ?? new FrontendOptions();

var allowedOrigins =
    ValidateAndNormalizeOrigins(
        frontendOptions ,
        isDevelopment);

builder.Services.Configure<FrontendOptions>(
    builder.Configuration.GetSection(
        FrontendOptions.SectionName));

var securityOptions =
    builder.Configuration
        .GetSection(ApiSecurityOptions.SectionName)
        .Get<ApiSecurityOptions>()
    ?? new ApiSecurityOptions();

ValidateSecurityOptions(
    securityOptions);

builder.Services.Configure<ApiSecurityOptions>(
    builder.Configuration.GetSection(
        ApiSecurityOptions.SectionName));

builder.WebHost.ConfigureKestrel(
    options =>
    {
        options.AddServerHeader = false;

        options.Limits.MaxRequestBodySize =
            securityOptions.MaximumRequestBodySizeBytes;
    });

builder.Services.Configure<FormOptions>(
    options =>
    {
        options.MultipartBodyLengthLimit =
            securityOptions.MaximumRequestBodySizeBytes;
    });

builder.Services.Configure<ForwardedHeadersOptions>(
    options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto;

        options.ForwardLimit = 1;

        if ( !isDevelopment )
        {
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        }
    });

builder.Services.AddHsts(
    options =>
    {
        options.MaxAge =
            TimeSpan.FromDays(180);
    });

const string FrontendCorsPolicy =
    "Frontend";

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            FrontendCorsPolicy ,
            policy =>
            {
                policy
                    .WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(
                        TimeSpan.FromHours(1));
            });
    });

builder.Services.AddRateLimiter(
    options =>
    {
        options.RejectionStatusCode =
            StatusCodes.Status429TooManyRequests;

        options.GlobalLimiter =
            PartitionedRateLimiter.Create<
                HttpContext ,
                string>(
                context =>
                {
                    if ( context.Connection.RemoteIpAddress is null )
                    {
                        return RateLimitPartition.GetNoLimiter(
                            "in-process-test-server");
                    }

                    var category =
                        GetRateLimitCategory(
                            context);

                    var permitLimit =
                        category switch
                        {
                            "authentication" =>
                                securityOptions
                                    .AuthenticationRequestsPerMinute,

                            "sensitive" =>
                                securityOptions
                                    .SensitiveRequestsPerMinute,

                            _ =>
                                securityOptions
                                    .GeneralRequestsPerMinute
                        };

                    var partitionKey =
                        $"{category}:" +
                        GetClientIdentifier(context);

                    return RateLimitPartition
                        .GetFixedWindowLimiter(
                            partitionKey ,
                            _ =>
                                new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit =
                                        permitLimit ,

                                    Window =
                                        TimeSpan.FromMinutes(1) ,

                                    QueueProcessingOrder =
                                        QueueProcessingOrder.OldestFirst ,

                                    QueueLimit =
                                        0 ,

                                    AutoReplenishment =
                                        true
                                });
                });

        options.OnRejected =
            async (
                context ,
                cancellationToken ) =>
            {
                context.HttpContext.Response.StatusCode =
                    StatusCodes.Status429TooManyRequests;

                context.HttpContext.Response.ContentType =
                    "application/problem+json";

                context.HttpContext.Response.Headers[
                    "Retry-After"] =
                    "60";

                var problemDetails =
                    new ProblemDetails
                    {
                        Status =
                            StatusCodes.Status429TooManyRequests ,

                        Title =
                            "Too many requests." ,

                        Detail =
                            "The request limit was exceeded. Try again later."
                    };

                problemDetails.Extensions["code"] =
                    "security.rate_limit_exceeded";

                problemDetails.Extensions["traceId"] =
                    Activity.Current?.Id ??
                    context.HttpContext.TraceIdentifier;

                await context.HttpContext.Response.WriteAsJsonAsync(
                    problemDetails ,
                    cancellationToken:
                        cancellationToken);
            };
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc(
            "v1" ,
            new OpenApiInfo
            {
                Title = "Mawasem API" ,
                Version = "v1"
            });

        options.AddSecurityDefinition(
            "Bearer" ,
            new OpenApiSecurityScheme
            {
                Name = "Authorization" ,
                Type = SecuritySchemeType.Http ,
                Scheme = "bearer" ,
                BearerFormat = "JWT" ,
                In = ParameterLocation.Header ,

                Description =
                    "Enter the JWT access token only. " +
                    "Do not include the word Bearer."
            });

        options.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference =
                            new OpenApiReference
                            {
                                Type =
                                    ReferenceType.SecurityScheme ,

                                Id =
                                    "Bearer"
                            }
                    } ,
                    Array.Empty<string>()
                }
            });
    });

var connectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection")
    ?? throw new InvalidOperationException(
        "The DefaultConnection connection string is missing.");

if ( !isDevelopment &&
    connectionString.Contains(
        "(localdb)" ,
        StringComparison.OrdinalIgnoreCase) )
{
    throw new InvalidOperationException(
        "Production cannot use SQL Server LocalDB. " +
        "Configure the Azure SQL connection string.");
}

var allowedHosts =
    builder.Configuration["AllowedHosts"];

if ( !isDevelopment &&
    ( string.IsNullOrWhiteSpace(allowedHosts) ||
        allowedHosts
            .Split(
                ';' ,
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries)
            .Any(host =>
                host == "*") ) )
{
    throw new InvalidOperationException(
        "Production AllowedHosts must contain explicit API host names.");
}

builder.Services.AddDbContext<MawasemDbContext>(
    options =>
    {
        options.UseSqlServer(
            connectionString);
    });

builder.Services.AddHealthChecks()
    .AddCheck(
        "self" ,
        () =>
            HealthCheckResult.Healthy(
                "The API process is running.") ,
        tags:
        [
            "live" ,
            "ready"
        ])
    .AddCheck<DatabaseHealthCheck>(
        "database" ,
        tags:
        [
            "ready"
        ]);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(
        JwtSettings.SectionName));

builder.Services.Configure<SuperAdminSeedOptions>(
    builder.Configuration.GetSection(
        SuperAdminSeedOptions.SectionName));

builder.Services.Configure<CustomerPasswordResetOptions>(
    builder.Configuration.GetSection(
        CustomerPasswordResetOptions.SectionName));

var jwtSettings =
    builder.Configuration
        .GetSection(JwtSettings.SectionName)
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "The JWT configuration section is missing.");

if ( string.IsNullOrWhiteSpace(jwtSettings.Issuer) )
{
    throw new InvalidOperationException(
        "Jwt:Issuer is required.");
}

if ( string.IsNullOrWhiteSpace(jwtSettings.Audience) )
{
    throw new InvalidOperationException(
        "Jwt:Audience is required.");
}

if ( string.IsNullOrWhiteSpace(jwtSettings.Key) )
{
    throw new InvalidOperationException(
        "Jwt:Key is required. Store it using User Secrets locally " +
        "and Azure Key Vault in production.");
}

byte[] jwtKeyBytes;

try
{
    jwtKeyBytes =
        Convert.FromBase64String(
            jwtSettings.Key);
}
catch ( FormatException exception )
{
    throw new InvalidOperationException(
        "Jwt:Key must be a valid Base64 value." ,
        exception);
}

if ( jwtKeyBytes.Length < 32 )
{
    throw new InvalidOperationException(
        "Jwt:Key must contain at least 32 bytes.");
}

if ( jwtSettings.AccessTokenMinutes <= 0 )
{
    throw new InvalidOperationException(
        "Jwt:AccessTokenMinutes must be greater than zero.");
}

if ( jwtSettings.RefreshTokenDays <= 0 )
{
    throw new InvalidOperationException(
        "Jwt:RefreshTokenDays must be greater than zero.");
}

builder.Services
    .AddIdentityCore<ApplicationUser>(
        options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            options.User.RequireUniqueEmail = false;

            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5;

            options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(15);
        })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<MawasemDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.MapInboundClaims = false;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true ,
                    ValidIssuer = jwtSettings.Issuer ,

                    ValidateAudience = true ,
                    ValidAudience = jwtSettings.Audience ,

                    ValidateIssuerSigningKey = true ,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            jwtKeyBytes) ,

                    ValidateLifetime = true ,
                    RequireExpirationTime = true ,

                    ClockSkew =
                        TimeSpan.FromSeconds(30) ,

                    NameClaimType =
                        JwtClaimNames.Name ,

                    RoleClaimType =
                        JwtClaimNames.Role
                };

            options.Events =
                new JwtBearerEvents
                {
                    OnMessageReceived =
                        context =>
                        {
                            if ( context.Request.Cookies.TryGetValue(
                                    AuthenticationCookieNames.AccessToken ,
                                    out var accessToken) )
                            {
                                context.Token =
                                    accessToken;
                            }

                            return Task.CompletedTask;
                        }
                };
        });

builder.Services.AddAuthorization(
    options =>
    {
        foreach ( var permission in SystemPermissions.All )
        {
            options.AddPolicy(
                permission ,
                policy =>
                {
                    policy.RequireAuthenticatedUser();

                    policy.AddRequirements(
                        new PermissionAuthorizationRequirement(
                            permission));
                });
        }
    });

builder.Services.AddScoped<
    IAuthorizationHandler ,
    PermissionAuthorizationHandler>();

builder.Services.AddSingleton(
    TimeProvider.System);

builder.Services.AddScoped<
    ITokenService ,
    JwtTokenService>();

builder.Services.AddScoped<
    ICustomerAuthenticationService ,
    CustomerAuthenticationService>();

builder.Services.AddScoped<
    ICustomerUserProfileService ,
    CustomerUserProfileService>();

builder.Services.AddScoped<
    ICustomerPasswordResetService ,
    CustomerPasswordResetService>();

if ( isDevelopment )
{
    builder.Services.AddScoped<
        ICustomerPasswordResetCodeSender ,
        DevelopmentCustomerPasswordResetCodeSender>();
}

builder.Services.AddScoped<
    IDashboardAuthenticationService ,
    DashboardAuthenticationService>();

builder.Services.AddScoped<
    IDashboardUserProfileService ,
    DashboardUserProfileService>();

builder.Services.AddScoped<
    IUserAddressService ,
    UserAddressService>();

builder.Services.AddScoped<
    ICheckoutService ,
    CheckoutService>();

builder.Services.AddScoped<
    IStoreOrderService ,
    StoreOrderService>();

builder.Services.AddScoped<
    IStorePickupCollectionService ,
    StorePickupCollectionService>();

builder.Services.AddScoped<
    IStoreReturnService ,
    StoreReturnService>();

builder.Services.AddScoped<
    IDeliveryAreaService ,
    DeliveryAreaService>();

builder.Services.AddScoped<
    IOrderQueryService ,
    OrderQueryService>();

builder.Services.AddScoped<
    IOrderWorkflowService ,
    OrderWorkflowService>();

builder.Services.AddScoped<
    IRefundRequestService ,
    RefundRequestService>();

builder.Services.AddScoped<
    IReviewService ,
    ReviewService>();

builder.Services.AddScoped<
    IReportService ,
    ReportService>();

builder.Services.AddScoped<
    IBrandManagementService ,
    BrandManagementService>();

builder.Services.AddScoped<
    ICategoryManagementService ,
    CategoryManagementService>();

builder.Services.AddScoped<
    ICollectionManagementService ,
    CollectionManagementService>();

builder.Services.AddScoped<
    IComplaintManagementService ,
    ComplaintManagementService>();

builder.Services.AddScoped<
    ICustomerManagementService ,
    CustomerManagementService>();

builder.Services.AddScoped<
    IEmployeeManagementService ,
    EmployeeManagementService>();

builder.Services.AddScoped<
    IProductManagementService ,
    ProductManagementService>();

builder.Services.AddScoped<
    IProductOptionManagementService ,
    ProductOptionManagementService>();

builder.Services.AddScoped<
    IProductVariantManagementService ,
    ProductVariantManagementService>();

builder.Services.AddScoped<
    IProductImageManagementService ,
    ProductImageManagementService>();

builder.Services.AddScoped<
    IPublicCatalogService ,
    PublicCatalogService>();

builder.Services.AddScoped<
    IRolePermissionManagementService ,
    RolePermissionManagementService>();

builder.Services.AddScoped<
    ISeasonManagementService ,
    SeasonManagementService>();

builder.Services.AddScoped<
    IdentityRoleSeeder>();

builder.Services.AddScoped<
    IdentityPermissionSeeder>();

builder.Services.AddScoped<
    FirstSuperAdminSeeder>();

if ( builder.Environment.IsDevelopment() )
{
    builder.Services.Configure<ProductImageStorageOptions>(
        options =>
        {
            options.RootPath =
                Path.Combine(
                    builder.Environment.ContentRootPath ,
                    "wwwroot" ,
                    "uploads" ,
                    "products");

            options.RequestPath =
                "/uploads/products";
        });

    builder.Services.AddSingleton<
        IProductImageStorage ,
        LocalProductImageStorage>();
}
else
{
    builder.Services
        .AddOptions<AzureBlobProductImageStorageOptions>()
        .Bind(
            builder.Configuration.GetSection(
                AzureBlobProductImageStorageOptions
                    .SectionName))
        .Validate(
            options =>
                Uri.TryCreate(
                    options.ServiceUri ,
                    UriKind.Absolute ,
                    out var serviceUri) &&
                serviceUri.Scheme ==
                    Uri.UriSchemeHttps ,
            "Azure Blob ServiceUri must be a valid HTTPS URI.")
        .Validate(
            options =>
                !string.IsNullOrWhiteSpace(
                    options.ContainerName) ,
            "Azure Blob ContainerName is required.")
        .Validate(
            options =>
                Uri.TryCreate(
                    options.PublicBaseUrl ,
                    UriKind.Absolute ,
                    out var publicBaseUri) &&
                publicBaseUri.Scheme ==
                    Uri.UriSchemeHttps ,
            "Azure Blob PublicBaseUrl must be a valid HTTPS URI.")
        .ValidateOnStart();

    builder.Services.AddSingleton<
        IProductImageStorage ,
        AzureBlobProductImageStorage>();
}

builder.Services.AddScoped<
    PendingProductImageDeletionProcessor>();

builder.Services.AddHostedService<
    PendingProductImageDeletionWorker>();

builder.Services.AddScoped<
    ICartService ,
    CartService>();

var app =
    builder.Build();

await using ( var scope =
    app.Services.CreateAsyncScope() )
{
    var roleSeeder =
        scope.ServiceProvider
            .GetRequiredService<IdentityRoleSeeder>();

    await roleSeeder.SeedAsync();

    var permissionSeeder =
        scope.ServiceProvider
            .GetRequiredService<IdentityPermissionSeeder>();

    await permissionSeeder.SeedAsync();

    var firstSuperAdminSeeder =
        scope.ServiceProvider
            .GetRequiredService<FirstSuperAdminSeeder>();

    await firstSuperAdminSeeder.SeedAsync();
}

app.UseForwardedHeaders();

if ( isDevelopment )
{
    app.UseSwagger();

    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseMiddleware<SecurityHeadersMiddleware>();

app.UseStaticFiles();

app.UseRouting();

app.UseCors(
    FrontendCorsPolicy);

app.UseMiddleware<
    UnsafeRequestOriginValidationMiddleware>();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapHealthChecks(
    "/health/live" ,
    new HealthCheckOptions
    {
        Predicate =
            registration =>
                registration.Tags.Contains(
                    "live") ,

        ResponseWriter =
            WriteHealthResponseAsync
    });

app.MapHealthChecks(
    "/health/ready" ,
    new HealthCheckOptions
    {
        Predicate =
            registration =>
                registration.Tags.Contains(
                    "ready") ,

        ResponseWriter =
            WriteHealthResponseAsync
    });

app.MapControllers();

app.Run();

static string[] ValidateAndNormalizeOrigins(
    FrontendOptions options ,
    bool isDevelopment )
{
    var origins =
        options.AllowedOrigins
            .Where(origin =>
                !string.IsNullOrWhiteSpace(origin))
            .Select(origin =>
                origin.Trim())
            .Distinct(
                StringComparer.OrdinalIgnoreCase)
            .ToArray();

    if ( origins.Length == 0 )
    {
        throw new InvalidOperationException(
            "At least one Frontend:AllowedOrigins value is required.");
    }

    var normalizedOrigins =
        new List<string>(
            origins.Length);

    foreach ( var origin in origins )
    {
        if ( !Uri.TryCreate(
                origin ,
                UriKind.Absolute ,
                out var uri) )
        {
            throw new InvalidOperationException(
                $"Frontend origin '{origin}' is not a valid absolute URL.");
        }

        var usesHttps =
            string.Equals(
                uri.Scheme ,
                Uri.UriSchemeHttps ,
                StringComparison.OrdinalIgnoreCase);

        var usesDevelopmentHttp =
            isDevelopment &&
            string.Equals(
                uri.Scheme ,
                Uri.UriSchemeHttp ,
                StringComparison.OrdinalIgnoreCase);

        if ( !usesHttps &&
            !usesDevelopmentHttp )
        {
            throw new InvalidOperationException(
                $"Frontend origin '{origin}' must use HTTPS.");
        }

        if ( !isDevelopment &&
            uri.IsLoopback )
        {
            throw new InvalidOperationException(
                "Production frontend origins cannot use localhost.");
        }

        if ( uri.AbsolutePath != "/" ||
            !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment) )
        {
            throw new InvalidOperationException(
                $"Frontend origin '{origin}' cannot contain a path, " +
                "query string, or fragment.");
        }

        normalizedOrigins.Add(
            uri.GetLeftPart(
                UriPartial.Authority));
    }

    return normalizedOrigins
        .Distinct(
            StringComparer.OrdinalIgnoreCase)
        .ToArray();
}

static void ValidateSecurityOptions(
    ApiSecurityOptions options )
{
    if ( options.GeneralRequestsPerMinute <= 0 )
    {
        throw new InvalidOperationException(
            "Security:GeneralRequestsPerMinute must be greater than zero.");
    }

    if ( options.AuthenticationRequestsPerMinute <= 0 )
    {
        throw new InvalidOperationException(
            "Security:AuthenticationRequestsPerMinute must be greater than zero.");
    }

    if ( options.SensitiveRequestsPerMinute <= 0 )
    {
        throw new InvalidOperationException(
            "Security:SensitiveRequestsPerMinute must be greater than zero.");
    }

    const long minimumBodySize =
        1024 * 1024;

    const long maximumBodySize =
        100 * 1024 * 1024;

    if ( options.MaximumRequestBodySizeBytes <
            minimumBodySize ||
        options.MaximumRequestBodySizeBytes >
            maximumBodySize )
    {
        throw new InvalidOperationException(
            "Security:MaximumRequestBodySizeBytes must be between " +
            "1 MB and 100 MB.");
    }
}

static string GetRateLimitCategory(
    HttpContext context )
{
    if ( context.Request.Path.StartsWithSegments(
            "/api/auth") ||
        context.Request.Path.StartsWithSegments(
            "/api/admin/auth") )
    {
        return "authentication";
    }

    if ( !HttpMethods.IsGet(context.Request.Method) &&
        !HttpMethods.IsHead(context.Request.Method) &&
        !HttpMethods.IsOptions(context.Request.Method) )
    {
        return "sensitive";
    }

    return "general";
}

static string GetClientIdentifier(
    HttpContext context )
{
    return context.Connection.RemoteIpAddress?
        .ToString()
        ?? "unknown";
}

static Task WriteHealthResponseAsync(
    HttpContext context ,
    HealthReport report )
{
    context.Response.ContentType =
        "application/json";

    var response =
        new
        {
            status =
                report.Status.ToString() ,

            checks =
                report.Entries
                    .OrderBy(entry =>
                        entry.Key)
                    .Select(
                        entry =>
                            new
                            {
                                name =
                                    entry.Key ,

                                status =
                                    entry.Value.Status.ToString() ,

                                durationMilliseconds =
                                    Math.Round(
                                        entry.Value.Duration.TotalMilliseconds ,
                                        2)
                            })
                    .ToArray() ,

            timestampUtc =
                DateTimeOffset.UtcNow.ToString(
                    "O" ,
                    CultureInfo.InvariantCulture)
        };

    return context.Response.WriteAsJsonAsync(
        response);
}

public partial class Program
{
}