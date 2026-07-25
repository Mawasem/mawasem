using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Application.Features.Refunds.Interfaces;
using Mawasem.Application.Features.Refunds.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/refund-requests")]
public sealed class AdminRefundRequestsController
    : ControllerBase
{
    private readonly IRefundRequestService
        _refundRequestService;

    public AdminRefundRequestsController(
        IRefundRequestService refundRequestService )
    {
        _refundRequestService =
            refundRequestService;
    }

    [HttpGet]
    [RequirePermission(SystemPermissions.Refunds.View)]
    [ProducesResponseType(
        typeof(AdminRefundRequestListResponse) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status500InternalServerError)]
    public async Task<
        ActionResult<AdminRefundRequestListResponse>>
        GetListAsync(
            [FromQuery] GetAdminRefundRequestsRequest request ,
            CancellationToken cancellationToken )
    {
        var result =
            await _refundRequestService.GetAdminListAsync(
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
            return UnexpectedResponseFailure();
        }

        return Ok(result.Response);
    }

    [HttpGet("{refundRequestId:int}")]
    [RequirePermission(SystemPermissions.Refunds.View)]
    [ProducesResponseType(
        typeof(AdminRefundRequestDetailsResponse) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status500InternalServerError)]
    public async Task<
        ActionResult<AdminRefundRequestDetailsResponse>>
        GetDetailsAsync(
            int refundRequestId ,
            CancellationToken cancellationToken )
    {
        var result =
            await _refundRequestService.GetAdminDetailsAsync(
                refundRequestId ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        if ( result.Response is null )
        {
            return UnexpectedResponseFailure();
        }

        return Ok(result.Response);
    }

    [HttpPut("{refundRequestId:int}/approve")]
    [RequirePermission(SystemPermissions.Refunds.Approve)]
    [ProducesResponseType(
        typeof(AdminRefundRequestDetailsResponse) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status500InternalServerError)]
    public async Task<
        ActionResult<AdminRefundRequestDetailsResponse>>
        ApproveAsync(
            int refundRequestId ,
            [FromBody] ApproveRefundRequestRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var dashboardUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _refundRequestService.ApproveAsync(
                refundRequestId ,
                dashboardUserId ,
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
            return UnexpectedResponseFailure();
        }

        return Ok(result.Response);
    }

    [HttpPut("{refundRequestId:int}/reject")]
    [RequirePermission(SystemPermissions.Refunds.Reject)]
    [ProducesResponseType(
        typeof(AdminRefundRequestDetailsResponse) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status500InternalServerError)]
    public async Task<
        ActionResult<AdminRefundRequestDetailsResponse>>
        RejectAsync(
            int refundRequestId ,
            [FromBody] RejectRefundRequestRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var dashboardUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _refundRequestService.RejectAsync(
                refundRequestId ,
                dashboardUserId ,
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
            return UnexpectedResponseFailure();
        }

        return Ok(result.Response);
    }

    [HttpPut("{refundRequestId:int}/complete")]
    [RequirePermission(SystemPermissions.Payments.Refund)]
    [ProducesResponseType(
        typeof(AdminRefundRequestDetailsResponse) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status403Forbidden)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status500InternalServerError)]
    public async Task<
        ActionResult<AdminRefundRequestDetailsResponse>>
        CompleteAsync(
            int refundRequestId ,
            [FromBody] CompleteRefundRequestRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var dashboardUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _refundRequestService.CompleteAsync(
                refundRequestId ,
                dashboardUserId ,
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
            return UnexpectedResponseFailure();
        }

        return Ok(result.Response);
    }

    private ObjectResult InvalidAuthenticationToken()
    {
        var problemDetails =
            new ProblemDetails
            {
                Status =
                    StatusCodes.Status401Unauthorized ,

                Title =
                    "Invalid authentication token." ,

                Detail =
                    "The authenticated dashboard user " +
                    "identifier is invalid."
            };

        problemDetails.Extensions["code"] =
            RefundRequestErrorCodes.InvalidRequest;

        return StatusCode(
            StatusCodes.Status401Unauthorized ,
            problemDetails);
    }

    private ObjectResult CreateFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode =
            errorCode switch
            {
                RefundRequestErrorCodes.InvalidRequest =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.InvalidAdminNotes =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.InvalidQuantity =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes
                    .RefundRequestNotFound =>
                    StatusCodes.Status404NotFound,

                RefundRequestErrorCodes
                    .InvalidStatusTransition =>
                    StatusCodes.Status409Conflict,

                RefundRequestErrorCodes
                    .QuantityExceedsRefundable =>
                    StatusCodes.Status409Conflict,

                RefundRequestErrorCodes
                    .ConcurrencyConflict =>
                    StatusCodes.Status409Conflict,

                RefundRequestErrorCodes.OperationFailed =>
                    StatusCodes.Status500InternalServerError,

                _ =>
                    StatusCodes.Status500InternalServerError
            };

        var problemDetails =
            new ProblemDetails
            {
                Status =
                    statusCode ,

                Title =
                    "Admin refund request operation failed." ,

                Detail =
                    errorMessage ??
                    "The admin refund request operation " +
                    "could not be completed."
            };

        problemDetails.Extensions["code"] =
            errorCode ??
            RefundRequestErrorCodes.OperationFailed;

        return StatusCode(
            statusCode ,
            problemDetails);
    }

    private ObjectResult UnexpectedResponseFailure()
    {
        var problemDetails =
            new ProblemDetails
            {
                Status =
                    StatusCodes.Status500InternalServerError ,

                Title =
                    "Admin refund response failed." ,

                Detail =
                    "The refund operation succeeded, but its " +
                    "response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "refunds.admin_response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}