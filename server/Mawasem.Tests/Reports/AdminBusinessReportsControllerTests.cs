using System.Reflection;
using Mawasem.API.Authorization;
using Mawasem.API.Controllers;
using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Contracts.Responses;
using Mawasem.Application.Features.Reports.Interfaces;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.Tests.Reports;

public sealed class AdminBusinessReportsControllerTests
{
    [Fact]
    public async Task
        GetDashboard_Success_ReturnsOkResponse()
    {
        var response =
            new BusinessDashboardResponse
            {
                TotalOrders =
                    10 ,

                DeliveredOrders =
                    4 ,

                GrossSales =
                    500m ,

                CompletedRefundAmount =
                    50m ,

                NetRevenue =
                    450m
            };

        var reportService =
            new StubReportService
            {
                BusinessDashboardResult =
                    ReportResult<
                        BusinessDashboardResponse>.Success(
                            response)
            };

        var controller =
            new AdminBusinessReportsController(
                reportService);

        var result =
            await controller.GetDashboard(
                new GetBusinessDashboardRequest() ,
                CancellationToken.None);

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);

        Assert.Same(
            response ,
            okResult.Value);
    }

    [Fact]
    public async Task
        GetDashboard_InvalidDateRange_ReturnsBadRequestProblem()
    {
        var reportService =
            new StubReportService
            {
                BusinessDashboardResult =
                    ReportResult<
                        BusinessDashboardResponse>.Failure(
                            ReportErrorCodes.InvalidDateRange ,
                            "The start date cannot be later than the end date.")
            };

        var controller =
            new AdminBusinessReportsController(
                reportService);

        var result =
            await controller.GetDashboard(
                new GetBusinessDashboardRequest() ,
                CancellationToken.None);

        var objectResult =
            Assert.IsType<ObjectResult>(
                result);

        Assert.Equal(
            StatusCodes.Status400BadRequest ,
            objectResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                objectResult.Value);

        Assert.Equal(
            StatusCodes.Status400BadRequest ,
            problemDetails.Status);

        Assert.Equal(
            "The start date cannot be later than the end date." ,
            problemDetails.Detail);

        Assert.Equal(
            ReportErrorCodes.InvalidDateRange ,
            Assert.IsType<string>(
                problemDetails.Extensions["code"]));
    }

    [Fact]
    public async Task
        GetSalesOverTime_Success_ReturnsOkResponse()
    {
        var response =
            new SalesOverTimeResponse
            {
                TotalDeliveredOrders =
                    3 ,

                TotalGrossSales =
                    600m ,

                TotalCompletedRefundAmount =
                    100m ,

                TotalNetRevenue =
                    500m
            };

        var reportService =
            new StubReportService
            {
                SalesOverTimeResult =
                    ReportResult<
                        SalesOverTimeResponse>.Success(
                            response)
            };

        var controller =
            new AdminBusinessReportsController(
                reportService);

        var result =
            await controller.GetSalesOverTime(
                new GetSalesOverTimeRequest() ,
                CancellationToken.None);

        var okResult =
            Assert.IsType<OkObjectResult>(
                result);

        Assert.Same(
            response ,
            okResult.Value);
    }

    [Fact]
    public async Task
        GetSalesOverTime_OperationFailed_ReturnsInternalServerErrorProblem()
    {
        var reportService =
            new StubReportService
            {
                SalesOverTimeResult =
                    ReportResult<
                        SalesOverTimeResponse>.Failure(
                            ReportErrorCodes.OperationFailed ,
                            "The sales report could not be generated.")
            };

        var controller =
            new AdminBusinessReportsController(
                reportService);

        var result =
            await controller.GetSalesOverTime(
                new GetSalesOverTimeRequest() ,
                CancellationToken.None);

        var objectResult =
            Assert.IsType<ObjectResult>(
                result);

        Assert.Equal(
            StatusCodes.Status500InternalServerError ,
            objectResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                objectResult.Value);

        Assert.Equal(
            StatusCodes.Status500InternalServerError ,
            problemDetails.Status);

        Assert.Equal(
            "The sales report could not be generated." ,
            problemDetails.Detail);

        Assert.Equal(
            ReportErrorCodes.OperationFailed ,
            Assert.IsType<string>(
                problemDetails.Extensions["code"]));
    }

    [Fact]
    public void
        Endpoints_HaveExpectedRoutesAndReportViewPermission()
    {
        var controllerType =
            typeof(AdminBusinessReportsController);

        var controllerRoute =
            controllerType.GetCustomAttribute<
                RouteAttribute>();

        Assert.NotNull(
            controllerRoute);

        Assert.Equal(
            "api/admin/reports" ,
            controllerRoute!.Template);

        var dashboardMethod =
            controllerType.GetMethod(
                nameof(
                    AdminBusinessReportsController
                        .GetDashboard));

        Assert.NotNull(
            dashboardMethod);

        var dashboardHttpGet =
            dashboardMethod!.GetCustomAttribute<
                HttpGetAttribute>();

        var dashboardPermission =
            dashboardMethod.GetCustomAttribute<
                RequirePermissionAttribute>();

        Assert.NotNull(
            dashboardHttpGet);

        Assert.Equal(
            "dashboard" ,
            dashboardHttpGet!.Template);

        Assert.NotNull(
            dashboardPermission);

        Assert.Equal(
            SystemPermissions.Reports.View ,
            dashboardPermission!.Policy);

        var salesMethod =
            controllerType.GetMethod(
                nameof(
                    AdminBusinessReportsController
                        .GetSalesOverTime));

        Assert.NotNull(
            salesMethod);

        var salesHttpGet =
            salesMethod!.GetCustomAttribute<
                HttpGetAttribute>();

        var salesPermission =
            salesMethod.GetCustomAttribute<
                RequirePermissionAttribute>();

        Assert.NotNull(
            salesHttpGet);

        Assert.Equal(
            "sales-over-time" ,
            salesHttpGet!.Template);

        Assert.NotNull(
            salesPermission);

        Assert.Equal(
            SystemPermissions.Reports.View ,
            salesPermission!.Policy);
    }

    private sealed class StubReportService
        : IReportService
    {
        public ReportResult<EmployeeReportSummaryResponse>
            EmployeeSummaryResult { get; init; } =
                ReportResult<
                    EmployeeReportSummaryResponse>.Success(
                        new EmployeeReportSummaryResponse());

        public ReportResult<EmployeeOrderActionsResponse>
            EmployeeOrderActionsResult { get; init; } =
                ReportResult<
                    EmployeeOrderActionsResponse>.Success(
                        new EmployeeOrderActionsResponse());

        public ReportResult<BusinessDashboardResponse>
            BusinessDashboardResult { get; init; } =
                ReportResult<
                    BusinessDashboardResponse>.Success(
                        new BusinessDashboardResponse());

        public ReportResult<SalesOverTimeResponse>
            SalesOverTimeResult { get; init; } =
                ReportResult<
                    SalesOverTimeResponse>.Success(
                        new SalesOverTimeResponse());

        public Task<
            ReportResult<EmployeeReportSummaryResponse>>
            GetEmployeeSummaryAsync(
                GetEmployeeReportRequest request ,
                CancellationToken cancellationToken = default )
        {
            return Task.FromResult(
                EmployeeSummaryResult);
        }

        public Task<
            ReportResult<EmployeeOrderActionsResponse>>
            GetEmployeeOrderActionsAsync(
                int employeeId ,
                GetEmployeeOrderActionsRequest request ,
                CancellationToken cancellationToken = default )
        {
            return Task.FromResult(
                EmployeeOrderActionsResult);
        }

        public Task<
            ReportResult<BusinessDashboardResponse>>
            GetBusinessDashboardAsync(
                GetBusinessDashboardRequest request ,
                CancellationToken cancellationToken = default )
        {
            return Task.FromResult(
                BusinessDashboardResult);
        }

        public Task<
            ReportResult<SalesOverTimeResponse>>
            GetSalesOverTimeAsync(
                GetSalesOverTimeRequest request ,
                CancellationToken cancellationToken = default )
        {
            return Task.FromResult(
                SalesOverTimeResult);
        }
    }
}
