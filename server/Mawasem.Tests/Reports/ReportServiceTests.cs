using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Reports;
using Mawasem.Tests.Checkout;
using Microsoft.AspNetCore.Identity;

namespace Mawasem.Tests.Reports;

public sealed class ReportServiceTests
{
    private const int DeliveryEmployeeId = 51;

    private const int DeliveryRoleId = 501;

    private static readonly DateTime BaseTimeUtc =
        new(
            2026 ,
            7 ,
            1 ,
            8 ,
            0 ,
            0 ,
            DateTimeKind.Utc);

    [Fact]
    public async Task
        GetEmployeeSummaryAsync_MultipleEmployees_GroupsOnlyDashboardActions()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedReportScenarioAsync(
            database);

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetEmployeeSummaryAsync(
                    new GetEmployeeReportRequest());

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        Assert.Equal(
            2 ,
            result.Response!.TotalCount);

        Assert.Equal(
            2 ,
            result.Response.Items.Count);

        var admin =
            Assert.Single(
                result.Response.Items.Where(item =>
                    item.EmployeeId ==
                    CheckoutTestDatabase.DashboardUserId));

        Assert.Contains(
            SystemRoles.Admin ,
            admin.Roles);

        Assert.Equal(
            3 ,
            admin.TotalOrderActions);

        Assert.Collection(
            admin.OrderActions ,
            action =>
            {
                Assert.Equal(
                    OrderStatus.Confirmed ,
                    action.ActionStatus);

                Assert.Equal(
                    1 ,
                    action.Count);
            } ,
            action =>
            {
                Assert.Equal(
                    OrderStatus.Preparing ,
                    action.ActionStatus);

                Assert.Equal(
                    1 ,
                    action.Count);
            } ,
            action =>
            {
                Assert.Equal(
                    OrderStatus.Cancelled ,
                    action.ActionStatus);

                Assert.Equal(
                    1 ,
                    action.Count);
            });

        var deliveryEmployee =
            Assert.Single(
                result.Response.Items.Where(item =>
                    item.EmployeeId ==
                    DeliveryEmployeeId));

        Assert.Contains(
            SystemRoles.DeliveryEmployee ,
            deliveryEmployee.Roles);

        Assert.Equal(
            2 ,
            deliveryEmployee.TotalOrderActions);

        Assert.Collection(
            deliveryEmployee.OrderActions ,
            action =>
            {
                Assert.Equal(
                    OrderStatus.Shipped ,
                    action.ActionStatus);

                Assert.Equal(
                    1 ,
                    action.Count);
            } ,
            action =>
            {
                Assert.Equal(
                    OrderStatus.Delivered ,
                    action.ActionStatus);

                Assert.Equal(
                    1 ,
                    action.Count);
            });
    }

    [Fact]
    public async Task
        GetEmployeeSummaryAsync_RoleStatusAndDateFilters_ReturnMatchingEmployee()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedReportScenarioAsync(
            database);

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetEmployeeSummaryAsync(
                    new GetEmployeeReportRequest
                    {
                        Role =
                            SystemRoles.DeliveryEmployee ,

                        ActionStatus =
                            OrderStatus.Delivered ,

                        FromDateUtc =
                            BaseTimeUtc.AddHours(2.5) ,

                        ToDateUtc =
                            BaseTimeUtc.AddHours(3.5)
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        Assert.Equal(
            1 ,
            result.Response!.TotalCount);

        var employee =
            Assert.Single(
                result.Response.Items);

        Assert.Equal(
            DeliveryEmployeeId ,
            employee.EmployeeId);

        Assert.Equal(
            1 ,
            employee.TotalOrderActions);

        var action =
            Assert.Single(
                employee.OrderActions);

        Assert.Equal(
            OrderStatus.Delivered ,
            action.ActionStatus);

        Assert.Equal(
            1 ,
            action.Count);
    }

    [Fact]
    public async Task
        GetEmployeeOrderActionsAsync_Admin_ReturnsExactActionsNewestFirst()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedReportScenarioAsync(
            database);

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetEmployeeOrderActionsAsync(
                    CheckoutTestDatabase.DashboardUserId ,
                    new GetEmployeeOrderActionsRequest());

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        Assert.Equal(
            3 ,
            result.Response!.TotalCount);

        Assert.Contains(
            SystemRoles.Admin ,
            result.Response.Roles);

        Assert.Collection(
            result.Response.Items ,
            action =>
            {
                Assert.Equal(
                    "REP-002" ,
                    action.OrderNumber);

                Assert.Equal(
                    OrderStatus.Pending ,
                    action.PreviousStatus);

                Assert.Equal(
                    OrderStatus.Cancelled ,
                    action.NewStatus);

                Assert.Equal(
                    "Customer unavailable" ,
                    action.Reason);

                Assert.Equal(
                    75m ,
                    action.TotalAmount);
            } ,
            action =>
            {
                Assert.Equal(
                    "REP-001" ,
                    action.OrderNumber);

                Assert.Equal(
                    OrderStatus.Confirmed ,
                    action.PreviousStatus);

                Assert.Equal(
                    OrderStatus.Preparing ,
                    action.NewStatus);
            } ,
            action =>
            {
                Assert.Equal(
                    "REP-001" ,
                    action.OrderNumber);

                Assert.Equal(
                    OrderStatus.Pending ,
                    action.PreviousStatus);

                Assert.Equal(
                    OrderStatus.Confirmed ,
                    action.NewStatus);
            });
    }

    [Fact]
    public async Task
        GetEmployeeOrderActionsAsync_SearchStatusAndDate_ReturnMatchingAction()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedReportScenarioAsync(
            database);

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetEmployeeOrderActionsAsync(
                    CheckoutTestDatabase.DashboardUserId ,
                    new GetEmployeeOrderActionsRequest
                    {
                        Search =
                            "REP-002" ,

                        ActionStatus =
                            OrderStatus.Cancelled ,

                        FromDateUtc =
                            BaseTimeUtc.AddHours(3.5) ,

                        ToDateUtc =
                            BaseTimeUtc.AddHours(4.5)
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        Assert.Equal(
            1 ,
            result.Response!.TotalCount);

        var action =
            Assert.Single(
                result.Response.Items);

        Assert.Equal(
            "REP-002" ,
            action.OrderNumber);

        Assert.Equal(
            OrderStatus.Cancelled ,
            action.NewStatus);

        Assert.Equal(
            "Customer unavailable" ,
            action.Reason);
    }

    [Fact]
    public async Task
        GetEmployeeOrderActionsAsync_CustomerAccount_ReturnsEmployeeNotFound()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetEmployeeOrderActionsAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new GetEmployeeOrderActionsRequest());

        Assert.False(
            result.Succeeded);

        Assert.Equal(
            ReportErrorCodes.EmployeeNotFound ,
            result.ErrorCode);

        Assert.Null(
            result.Response);
    }

    private static async Task SeedReportScenarioAsync(
        CheckoutTestDatabase database )
    {
        await using var dbContext =
            database.CreateContext();

        var deliveryEmployee =
            new ApplicationUser
            {
                Id =
                    DeliveryEmployeeId ,

                UserName =
                    "delivery-report-employee" ,

                NormalizedUserName =
                    "DELIVERY-REPORT-EMPLOYEE" ,

                Email =
                    "delivery.report@mawasem.test" ,

                NormalizedEmail =
                    "DELIVERY.REPORT@MAWASEM.TEST" ,

                PhoneNumber =
                    "01000000051" ,

                FullNameAr =
                    "موظف توصيل التقارير" ,

                FullNameEn =
                    "Delivery Report Employee" ,

                SecurityStamp =
                    Guid.NewGuid().ToString()
            };

        var deliveryRole =
            new ApplicationRole
            {
                Id =
                    DeliveryRoleId ,

                Name =
                    SystemRoles.DeliveryEmployee ,

                NormalizedName =
                    SystemRoles.DeliveryEmployee
                        .ToUpperInvariant() ,

                ConcurrencyStamp =
                    Guid.NewGuid().ToString()
            };

        dbContext.Users.Add(
            deliveryEmployee);

        dbContext.Roles.Add(
            deliveryRole);

        dbContext.UserRoles.Add(
            new IdentityUserRole<int>
            {
                UserId =
                    DeliveryEmployeeId ,

                RoleId =
                    DeliveryRoleId
            });

        var deliveredOrder =
            CreateOrder(
                "REP-001" ,
                OrderStatus.Delivered ,
                200m ,
                BaseTimeUtc);

        var cancelledOrder =
            CreateOrder(
                "REP-002" ,
                OrderStatus.Cancelled ,
                75m ,
                BaseTimeUtc.AddMinutes(1));

        var deletedOrder =
            CreateOrder(
                "REP-003" ,
                OrderStatus.Delivered ,
                120m ,
                BaseTimeUtc.AddMinutes(2) ,
                isDeleted: true);

        var customerCancelledOrder =
            CreateOrder(
                "REP-004" ,
                OrderStatus.Cancelled ,
                50m ,
                BaseTimeUtc.AddMinutes(3));

        dbContext.Orders.AddRange(
            deliveredOrder ,
            cancelledOrder ,
            deletedOrder ,
            customerCancelledOrder);

        await dbContext.SaveChangesAsync();

        dbContext.OrderStatusHistories.AddRange(
            CreateHistory(
                deliveredOrder ,
                OrderStatus.Pending ,
                OrderStatus.Confirmed ,
                CheckoutTestDatabase.DashboardUserId ,
                OrderStatusChangeActorType.DashboardUser ,
                BaseTimeUtc) ,

            CreateHistory(
                deliveredOrder ,
                OrderStatus.Confirmed ,
                OrderStatus.Preparing ,
                CheckoutTestDatabase.DashboardUserId ,
                OrderStatusChangeActorType.DashboardUser ,
                BaseTimeUtc.AddHours(1)) ,

            CreateHistory(
                deliveredOrder ,
                OrderStatus.Preparing ,
                OrderStatus.Shipped ,
                DeliveryEmployeeId ,
                OrderStatusChangeActorType.DashboardUser ,
                BaseTimeUtc.AddHours(2)) ,

            CreateHistory(
                deliveredOrder ,
                OrderStatus.Shipped ,
                OrderStatus.Delivered ,
                DeliveryEmployeeId ,
                OrderStatusChangeActorType.DashboardUser ,
                BaseTimeUtc.AddHours(3)) ,

            CreateHistory(
                cancelledOrder ,
                OrderStatus.Pending ,
                OrderStatus.Cancelled ,
                CheckoutTestDatabase.DashboardUserId ,
                OrderStatusChangeActorType.DashboardUser ,
                BaseTimeUtc.AddHours(4) ,
                "Customer unavailable") ,

            CreateHistory(
                deletedOrder ,
                OrderStatus.Pending ,
                OrderStatus.Confirmed ,
                CheckoutTestDatabase.DashboardUserId ,
                OrderStatusChangeActorType.DashboardUser ,
                BaseTimeUtc.AddHours(5)) ,

            CreateHistory(
                customerCancelledOrder ,
                OrderStatus.Pending ,
                OrderStatus.Cancelled ,
                CheckoutTestDatabase.CustomerId ,
                OrderStatusChangeActorType.Customer ,
                BaseTimeUtc.AddHours(6) ,
                "Customer changed their mind"));

        await dbContext.SaveChangesAsync();
    }

    private static Order CreateOrder(
        string orderNumber ,
        OrderStatus status ,
        decimal totalAmount ,
        DateTime orderDateUtc ,
        bool isDeleted = false )
    {
        return new Order
        {
            UserId =
                CheckoutTestDatabase.CustomerId ,

            CustomerNameAr =
                "عميل التقارير" ,

            CustomerNameEn =
                "Report Customer" ,

            CustomerPhone =
                "01000000001" ,

            OrderNumber =
                orderNumber ,

            OrderDate =
                orderDateUtc ,

            SubTotal =
                totalAmount ,

            Discount =
                0m ,

            DeliveryFee =
                0m ,

            TotalAmount =
                totalAmount ,

            OrderStatus =
                status ,

            PaymentMethod =
                PaymentMethod.CashOnDelivery ,

            PaymentStatus =
                PaymentStatus.Pending ,

            DeliveryMethod =
                DeliveryMethod.HomeDelivery ,

            OrderSource =
                OrderSource.Website ,

            CreatedOn =
                new DateTimeOffset(
                    orderDateUtc) ,

            CreatedBy =
                "report-test" ,

            IsDeleted =
                isDeleted ,

            DeletedOn =
                isDeleted
                    ? new DateTimeOffset(
                        orderDateUtc.AddHours(1))
                    : null ,

            DeletedBy =
                isDeleted
                    ? "report-test"
                    : null
        };
    }

    private static OrderStatusHistory CreateHistory(
        Order order ,
        OrderStatus previousStatus ,
        OrderStatus newStatus ,
        int changedByUserId ,
        OrderStatusChangeActorType actorType ,
        DateTime changedAtUtc ,
        string? reason = null )
    {
        return new OrderStatusHistory
        {
            OrderId =
                order.Id ,

            PreviousStatus =
                previousStatus ,

            NewStatus =
                newStatus ,

            ChangedByUserId =
                changedByUserId ,

            ActorType =
                actorType ,

            ChangedAtUtc =
                changedAtUtc ,

            Reason =
                reason
        };
    }
}
