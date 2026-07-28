using Mawasem.Application.Features.Complaints.Contracts.Requests;
using Mawasem.Application.Features.Complaints.Models;
using Mawasem.Domain.Identity;
using Mawasem.Infrastructure.Complaints;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mawasem.Tests.Complaints;

public sealed class ComplaintManagementServiceTests
{
    [Fact]
    public async Task
        CreateAsync_NormalizesInputAndStoresEmployeeAudit()
    {
        var timeProvider =
            new TestTimeProvider(
                new DateTimeOffset(
                    2026 ,
                    7 ,
                    28 ,
                    8 ,
                    30 ,
                    0 ,
                    TimeSpan.Zero));

        await using var provider =
            CreateServiceProvider(
                timeProvider);

        await using var scope =
            provider.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<MawasemDbContext>();

        var employee =
            new ApplicationUser
            {
                Id = 42 ,
                UserName =
                    "support@example.com" ,
                FullNameEn =
                    "Support Employee" ,
                FullNameAr =
                    "موظف الدعم"
            };

        dbContext.Users.Add(employee);

        await dbContext.SaveChangesAsync();

        var service =
            scope.ServiceProvider
                .GetRequiredService<
                    ComplaintManagementService>();

        var result =
            await service.CreateAsync(
                actorUserId: employee.Id ,
                new CreateComplaintRequest
                {
                    CustomerName =
                        "  Ahmed Ali  " ,
                    CustomerPhone =
                        "0100 123 4567" ,
                    ComplaintText =
                        "  Customer reported a delivery issue.  "
                });

        Assert.True(
            result.Succeeded);

        Assert.NotNull(
            result.Response);

        Assert.Equal(
            "Ahmed Ali" ,
            result.Response.CustomerName);

        Assert.Equal(
            "0100 123 4567" ,
            result.Response.CustomerPhone);

        Assert.Equal(
            "Customer reported a delivery issue." ,
            result.Response.ComplaintText);

        Assert.Equal(
            employee.Id ,
            result.Response.CreatedByEmployeeId);

        Assert.Equal(
            "Support Employee" ,
            result.Response.CreatedByEmployeeNameEn);

        Assert.Equal(
            "موظف الدعم" ,
            result.Response.CreatedByEmployeeNameAr);

        Assert.Equal(
            timeProvider.GetUtcNow() ,
            result.Response.CreatedOn);

        var storedComplaint =
            await dbContext.Complaints
                .SingleAsync();

        Assert.Equal(
            employee.Id ,
            storedComplaint.CreatedByEmployeeId);

        Assert.Equal(
            "42" ,
            storedComplaint.CreatedBy);

        Assert.Equal(
            timeProvider.GetUtcNow() ,
            storedComplaint.CreatedOn);

        Assert.False(
            storedComplaint.IsDeleted);
    }

    [Fact]
    public async Task
        GetListAsync_AppliesSearchEmployeeFilterAndPagination()
    {
        var timeProvider =
            new TestTimeProvider(
                new DateTimeOffset(
                    2026 ,
                    7 ,
                    28 ,
                    9 ,
                    0 ,
                    0 ,
                    TimeSpan.Zero));

        await using var provider =
            CreateServiceProvider(
                timeProvider);

        await using var scope =
            provider.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<MawasemDbContext>();

        var firstEmployee =
            new ApplicationUser
            {
                Id = 10 ,
                UserName =
                    "support-one@example.com" ,
                FullNameEn =
                    "Support One" ,
                FullNameAr =
                    "موظف دعم واحد"
            };

        var secondEmployee =
            new ApplicationUser
            {
                Id = 20 ,
                UserName =
                    "support-two@example.com" ,
                FullNameEn =
                    "Support Two" ,
                FullNameAr =
                    "موظف دعم اثنان"
            };

        dbContext.Users.AddRange(
            firstEmployee ,
            secondEmployee);

        await dbContext.SaveChangesAsync();

        var service =
            scope.ServiceProvider
                .GetRequiredService<
                    ComplaintManagementService>();

        await service.CreateAsync(
            firstEmployee.Id ,
            new CreateComplaintRequest
            {
                CustomerName = "Ahmed" ,
                CustomerPhone = "01001234567" ,
                ComplaintText =
                    "Delivery arrived late."
            });

        await service.CreateAsync(
            firstEmployee.Id ,
            new CreateComplaintRequest
            {
                CustomerName = "Mona" ,
                CustomerPhone = "01111234567" ,
                ComplaintText =
                    "Blanket package was damaged."
            });

        await service.CreateAsync(
            secondEmployee.Id ,
            new CreateComplaintRequest
            {
                CustomerName = "Omar" ,
                CustomerPhone = "01221234567" ,
                ComplaintText =
                    "Customer asked about the order."
            });

        var searchResult =
            await service.GetListAsync(
                new GetComplaintsRequest
                {
                    Search = "Blanket"
                });

        Assert.True(
            searchResult.Succeeded);

        Assert.NotNull(
            searchResult.Response);

        var searchItem =
            Assert.Single(
                searchResult.Response.Items);

        Assert.Equal(
            "Mona" ,
            searchItem.CustomerName);

        var employeeResult =
            await service.GetListAsync(
                new GetComplaintsRequest
                {
                    CreatedByEmployeeId =
                        secondEmployee.Id
                });

        Assert.True(
            employeeResult.Succeeded);

        Assert.NotNull(
            employeeResult.Response);

        var employeeItem =
            Assert.Single(
                employeeResult.Response.Items);

        Assert.Equal(
            secondEmployee.Id ,
            employeeItem.CreatedByEmployeeId);

        var pageResult =
            await service.GetListAsync(
                new GetComplaintsRequest
                {
                    PageNumber = 2 ,
                    PageSize = 2
                });

        Assert.True(
            pageResult.Succeeded);

        Assert.NotNull(
            pageResult.Response);

        Assert.Equal(
            3 ,
            pageResult.Response.TotalCount);

        Assert.Equal(
            2 ,
            pageResult.Response.TotalPages);

        Assert.Single(
            pageResult.Response.Items);
    }
    [Fact]
    public async Task
        GetByIdAsync_MissingComplaint_ReturnsNotFound()
    {
        var timeProvider =
            new TestTimeProvider(
                new DateTimeOffset(
                    2026 ,
                    7 ,
                    28 ,
                    10 ,
                    0 ,
                    0 ,
                    TimeSpan.Zero));

        await using var provider =
            CreateServiceProvider(
                timeProvider);

        await using var scope =
            provider.CreateAsyncScope();

        var service =
            scope.ServiceProvider
                .GetRequiredService<
                    ComplaintManagementService>();

        var result =
            await service.GetByIdAsync(
                complaintId: 999);

        Assert.False(
            result.Succeeded);

        Assert.Null(
            result.Response);

        Assert.Equal(
            ComplaintManagementErrorCodes.NotFound ,
            result.ErrorCode);
    }

    [Fact]
    public async Task
        InvalidRequests_ReturnInvalidRequest()
    {
        var timeProvider =
            new TestTimeProvider(
                new DateTimeOffset(
                    2026 ,
                    7 ,
                    28 ,
                    10 ,
                    30 ,
                    0 ,
                    TimeSpan.Zero));

        await using var provider =
            CreateServiceProvider(
                timeProvider);

        await using var scope =
            provider.CreateAsyncScope();

        var service =
            scope.ServiceProvider
                .GetRequiredService<
                    ComplaintManagementService>();

        var invalidPageResult =
            await service.GetListAsync(
                new GetComplaintsRequest
                {
                    PageSize = 101
                });

        Assert.False(
            invalidPageResult.Succeeded);

        Assert.Equal(
            ComplaintManagementErrorCodes.InvalidRequest ,
            invalidPageResult.ErrorCode);

        var invalidCreateResult =
            await service.CreateAsync(
                actorUserId: 1 ,
                new CreateComplaintRequest
                {
                    CustomerName = "   " ,
                    CustomerPhone = "01001234567" ,
                    ComplaintText =
                        "Complaint text."
                });

        Assert.False(
            invalidCreateResult.Succeeded);

        Assert.Equal(
            ComplaintManagementErrorCodes.InvalidRequest ,
            invalidCreateResult.ErrorCode);
    }
    private static ServiceProvider CreateServiceProvider(
        TestTimeProvider timeProvider )
    {
        var services =
            new ServiceCollection();

        services.AddLogging();

        services.AddDbContext<MawasemDbContext>(
            options =>
            {
                options.UseInMemoryDatabase(
                    Guid.NewGuid().ToString("N"));
            });

        services.AddSingleton<TimeProvider>(
            timeProvider);

        services.AddScoped<
            ComplaintManagementService>();

        return services.BuildServiceProvider();
    }

    private sealed class TestTimeProvider
        : TimeProvider
    {
        private readonly DateTimeOffset _utcNow;

        public TestTimeProvider(
            DateTimeOffset utcNow )
        {
            _utcNow = utcNow;
        }

        public override DateTimeOffset GetUtcNow()
        {
            return _utcNow;
        }
    }
}
