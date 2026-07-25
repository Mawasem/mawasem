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
[Route("api/orders/{orderId:int}/refund-requests")]
[Authorize(Roles = SystemRoles.Customer)]
public sealed class CustomerRefundRequestsController
    : ControllerBase
{
    private readonly IRefundRequestService
        _refundRequestService;

    public CustomerRefundRequestsController(
        IRefundRequestService refundRequestService )
    {
        _refundRequestService =
            refundRequestService;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(RefundRequestDetailsResponse) ,
        StatusCodes.Status201Created)]
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
        ActionResult<RefundRequestDetailsResponse>>
        CreateAsync(
            int orderId ,
            [FromBody] CreateRefundRequestRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var customerUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _refundRequestService.CreateAsync(
                orderId ,
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

        return StatusCode(
            StatusCodes.Status201Created ,
            result.Response);
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
            RefundRequestErrorCodes.OrderAccessDenied;

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
                RefundRequestErrorCodes.InvalidIdempotencyKey =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.InvalidCustomerReason =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.ItemsRequired =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.DuplicateOrderItem =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.OrderItemNotFound =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.InvalidQuantity =>
                    StatusCodes.Status400BadRequest,

                RefundRequestErrorCodes.OrderNotFound =>
                    StatusCodes.Status404NotFound,

                RefundRequestErrorCodes.OrderAccessDenied =>
                    StatusCodes.Status403Forbidden,

                RefundRequestErrorCodes.OrderNotDelivered =>
                    StatusCodes.Status409Conflict,

                RefundRequestErrorCodes.QuantityExceedsRefundable =>
                    StatusCodes.Status409Conflict,

                RefundRequestErrorCodes.InvalidStatusTransition =>
                    StatusCodes.Status409Conflict,

                RefundRequestErrorCodes.ConcurrencyConflict =>
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
                    "Refund request creation failed." ,

                Detail =
                    errorMessage ??
                    "The refund request could not be created."
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
                    "The refund request succeeded, but its " +
                    "response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "refunds.response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}