using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.Complaints.Contracts.Requests;
using Mawasem.Application.Features.Complaints.Interfaces;
using Mawasem.Application.Features.Complaints.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/complaints")]
public sealed class AdminComplaintsController
    : ControllerBase
{
    private readonly IComplaintManagementService
        _complaintManagementService;

    public AdminComplaintsController(
        IComplaintManagementService complaintManagementService )
    {
        _complaintManagementService =
            complaintManagementService;
    }

    [RequirePermission(
        SystemPermissions.Complaints.View)]
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] GetComplaintsRequest request ,
        CancellationToken cancellationToken )
    {
        var result =
            await _complaintManagementService
                .GetListAsync(
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

        return Ok(result.Response);
    }

    [RequirePermission(
        SystemPermissions.Complaints.View)]
    [HttpGet("{complaintId:int}")]
    public async Task<IActionResult> GetById(
        int complaintId ,
        CancellationToken cancellationToken )
    {
        var result =
            await _complaintManagementService
                .GetByIdAsync(
                    complaintId ,
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

        return Ok(result.Response);
    }

    [RequirePermission(
        SystemPermissions.Complaints.Create)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateComplaintRequest request ,
        CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var actorUserId) )
        {
            return CreateInvalidActorResponse();
        }

        var result =
            await _complaintManagementService
                .CreateAsync(
                    actorUserId ,
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

        return CreatedAtAction(
            nameof(GetById) ,
            new
            {
                complaintId =
                    result.Response.Id
            } ,
            result.Response);
    }

    private IActionResult CreateFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode =
            errorCode switch
            {
                ComplaintManagementErrorCodes.NotFound =>
                    StatusCodes.Status404NotFound,

                _ =>
                    StatusCodes.Status400BadRequest
            };

        var problemDetails =
            new ProblemDetails
            {
                Status =
                    statusCode ,

                Title =
                    "Complaint management request failed." ,

                Detail =
                    errorMessage
                    ?? "The complaint management request could not be completed."
            };

        problemDetails.Extensions["code"] =
            errorCode
            ?? ComplaintManagementErrorCodes.InvalidRequest;

        return StatusCode(
            statusCode ,
            problemDetails);
    }

    private IActionResult CreateInvalidActorResponse()
    {
        var problemDetails =
            new ProblemDetails
            {
                Status =
                    StatusCodes.Status401Unauthorized ,

                Title =
                    "Complaint management authentication failed." ,

                Detail =
                    "The authenticated dashboard account is invalid."
            };

        problemDetails.Extensions["code"] =
            ComplaintManagementErrorCodes.InvalidRequest;

        return Unauthorized(problemDetails);
    }

    private IActionResult CreateUnexpectedFailureResponse()
    {
        var problemDetails =
            new ProblemDetails
            {
                Status =
                    StatusCodes.Status500InternalServerError ,

                Title =
                    "Complaint management response failed." ,

                Detail =
                    "The complaint operation succeeded, but its response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "complaints.response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}
