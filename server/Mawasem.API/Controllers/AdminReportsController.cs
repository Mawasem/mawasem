using Mawasem.API.Authorization;
using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Interfaces;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/reports/employees")]
public sealed class AdminReportsController : ControllerBase
{
    private readonly IReportService
        _reportService;

    public AdminReportsController(
        IReportService reportService )
    {
        _reportService =
            reportService
            ?? throw new ArgumentNullException(
                nameof(reportService));
    }

    [RequirePermission(
        SystemPermissions.Reports.View)]
    [HttpGet]
    public async Task<IActionResult> GetEmployeeSummary(
        [FromQuery] GetEmployeeReportRequest request ,
        CancellationToken cancellationToken )
    {
        var result =
            await _reportService
                .GetEmployeeSummaryAsync(
                    request ,
                    cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        if ( result.Response is null )
        {
            return CreateUnexpectedFailureResponse();
        }

        return Ok(
            result.Response);
    }

    [RequirePermission(
        SystemPermissions.Reports.View)]
    [HttpGet("{employeeId:int}/order-actions")]
    public async Task<IActionResult> GetEmployeeOrderActions(
        int employeeId ,
        [FromQuery] GetEmployeeOrderActionsRequest request ,
        CancellationToken cancellationToken )
    {
        var result =
            await _reportService
                .GetEmployeeOrderActionsAsync(
                    employeeId ,
                    request ,
                    cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        if ( result.Response is null )
        {
            return CreateUnexpectedFailureResponse();
        }

        return Ok(
            result.Response);
    }

    private IActionResult CreateFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode =
            errorCode switch
            {
                ReportErrorCodes.EmployeeNotFound =>
                    StatusCodes.Status404NotFound,

                ReportErrorCodes.OperationFailed =>
                    StatusCodes.Status500InternalServerError,

                ReportErrorCodes.InvalidRequest =>
                    StatusCodes.Status400BadRequest,

                ReportErrorCodes.InvalidDateRange =>
                    StatusCodes.Status400BadRequest,

                _ =>
                    StatusCodes.Status400BadRequest
            };

        var problemDetails =
            new ProblemDetails
            {
                Status =
                    statusCode ,

                Title =
                    "Report request failed." ,

                Detail =
                    errorMessage
                    ?? "The report request could not be completed."
            };

        problemDetails.Extensions["code"] =
            errorCode
            ?? ReportErrorCodes.InvalidRequest;

        return StatusCode(
            statusCode ,
            problemDetails);
    }

    private IActionResult CreateUnexpectedFailureResponse()
    {
        var problemDetails =
            new ProblemDetails
            {
                Status =
                    StatusCodes.Status500InternalServerError ,

                Title =
                    "Report response failed." ,

                Detail =
                    "The report operation succeeded, but its response could not be returned."
            };

        problemDetails.Extensions["code"] =
            ReportErrorCodes.OperationFailed;

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}
