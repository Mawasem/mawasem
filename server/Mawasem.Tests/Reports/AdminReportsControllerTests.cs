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

public sealed class AdminReportsControllerTests
{
    [Fact]
    public async Task
        GetEmployeeSummary_Success_ReturnsOkResponse()
    {
        var response =
            new EmployeeReportSummaryResponse
            {
                Items =
                    Array.Empty<
                        EmployeeReportSummaryItemResponse>() ,

                PageNumber =
                    1 ,

                PageSize =
                    20 ,

                TotalCount =
                    0 ,

                TotalPages =
                    0
            };

        var reportService =
            new StubReportService
            {
                EmployeeSummaryResult =
                    ReportResult<
                        EmployeeReportSummaryResponse>.Success(
                            response)
            };

        var controller =
            new AdminReportsController(
                reportService);

        var result =
            await controller.GetEmployeeSummary(
                new GetEmployeeReportRequest() ,
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
        GetEmployeeSummary_InvalidRequest_ReturnsBadRequestProblem()
    {
        var reportService =
            new StubReportService
            {
                EmployeeSummaryResult =
                    ReportResult<
                        EmployeeReportSummaryResponse>.Failure(
                            ReportErrorCodes.InvalidRequest ,
                            "Page size is invalid.")
            };

        var controller =
            new AdminReportsController(
                reportService);

        var result =
            await controller.GetEmployeeSummary(
                new GetEmployeeReportRequest() ,
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
            "Page size is invalid." ,
            problemDetails.Detail);

        Assert.Equal(
            ReportErrorCodes.InvalidRequest ,
            Assert.IsType<string>(
                problemDetails.Extensions["code"]));
    }

    [Fact]
    public async Task
        GetEmployeeOrderActions_EmployeeNotFound_ReturnsNotFoundProblem()
    {
        var reportService =
            new StubReportService
            {
                EmployeeOrderActionsResult =
                    ReportResult<
                        EmployeeOrderActionsResponse>.Failure(
                            ReportErrorCodes.EmployeeNotFound ,
                            "The dashboard employee was not found.")
            };

        var controller =
            new AdminReportsController(
                reportService);

        var result =
            await controller.GetEmployeeOrderActions(
                999 ,
                new GetEmployeeOrderActionsRequest() ,
                CancellationToken.None);

        var objectResult =
            Assert.IsType<ObjectResult>(
                result);

        Assert.Equal(
            StatusCodes.Status404NotFound ,
            objectResult.StatusCode);

        var problemDetails =
            Assert.IsType<ProblemDetails>(
                objectResult.Value);

        Assert.Equal(
            ReportErrorCodes.EmployeeNotFound ,
            Assert.IsType<string>(
                problemDetails.Extensions["code"]));
    }

    [Fact]
    public void
        Endpoints_HaveExpectedRoutesAndReportViewPermission()
    {
        var controllerType =
            typeof(AdminReportsController);

        var controllerRoute =
            controllerType.GetCustomAttribute<
                RouteAttribute>();

        Assert.NotNull(
            controllerRoute);

        Assert.Equal(
            "api/admin/reports/employees" ,
            controllerRoute!.Template);

        var summaryMethod =
            controllerType.GetMethod(
                nameof(
                    AdminReportsController
                        .GetEmployeeSummary));

        Assert.NotNull(
            summaryMethod);

        var summaryHttpGet =
            summaryMethod!.GetCustomAttribute<
                HttpGetAttribute>();

        var summaryPermission =
            summaryMethod.GetCustomAttribute<
                RequirePermissionAttribute>();

        Assert.NotNull(
            summaryHttpGet);

        Assert.Null(
            summaryHttpGet!.Template);

        Assert.NotNull(
            summaryPermission);

        Assert.Equal(
            SystemPermissions.Reports.View ,
            summaryPermission!.Policy);

        var actionsMethod =
            controllerType.GetMethod(
                nameof(
                    AdminReportsController
                        .GetEmployeeOrderActions));

        Assert.NotNull(
            actionsMethod);

        var actionsHttpGet =
            actionsMethod!.GetCustomAttribute<
                HttpGetAttribute>();

        var actionsPermission =
            actionsMethod.GetCustomAttribute<
                RequirePermissionAttribute>();

        Assert.NotNull(
            actionsHttpGet);

        Assert.Equal(
            "{employeeId:int}/order-actions" ,
            actionsHttpGet!.Template);

        Assert.NotNull(
            actionsPermission);

        Assert.Equal(
            SystemPermissions.Reports.View ,
            actionsPermission!.Policy);
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
    }
}
