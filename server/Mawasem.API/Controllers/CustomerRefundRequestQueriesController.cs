using Mawasem.API.Extensions;
using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Application.Features.Refunds.Interfaces;
using Mawasem.Application.Features.Refunds.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/refund-requests")]
[Authorize(Roles = SystemRoles.Customer)]
public sealed class CustomerRefundRequestQueriesController
    : ControllerBase
{
    private readonly IRefundRequestService
        _refundRequestService;

    public CustomerRefundRequestQueriesController(
        IRefundRequestService refundRequestService )
    {
        _refundRequestService =
            refundRequestService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(CustomerRefundRequestListResponse) ,
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
        ActionResult<CustomerRefundRequestListResponse>>
        GetListAsync(
            [FromQuery] GetCustomerRefundRequestsRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var customerUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _refundRequestService.GetCustomerListAsync(
                customerUserId ,
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
    [ProducesResponseType(
        typeof(RefundRequestDetailsResponse) ,
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
        ActionResult<RefundRequestDetailsResponse>>
        GetDetailsAsync(
            int refundRequestId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var customerUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _refundRequestService.GetCustomerDetailsAsync(
                customerUserId ,
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
                    "The authenticated customer identifier is invalid."
            };

        problemDetails.Extensions["code"] =
            RefundRequestErrorCodes.CustomerNotFound;

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

                RefundRequestErrorCodes.CustomerNotFound =>
                    StatusCodes.Status401Unauthorized,

                RefundRequestErrorCodes.CustomerBlocked =>
                    StatusCodes.Status403Forbidden,

                RefundRequestErrorCodes.RefundRequestNotFound =>
                    StatusCodes.Status404NotFound,

                RefundRequestErrorCodes
                    .RefundRequestAccessDenied =>
                    StatusCodes.Status403Forbidden,

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
                    "Customer refund request query failed." ,

                Detail =
                    errorMessage ??
                    "The refund request query could not be completed."
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
                    "Refund request response failed." ,

                Detail =
                    "The refund request query succeeded, but its " +
                    "response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "refunds.response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}