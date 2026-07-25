using Mawasem.API.Extensions;
using Mawasem.Application.Features.Orders.Contracts.Requests;
using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Interfaces;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize(Roles = SystemRoles.Customer)]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderQueryService
        _orderQueryService;

    private readonly IOrderWorkflowService
        _orderWorkflowService;

    public OrdersController(
        IOrderQueryService orderQueryService ,
        IOrderWorkflowService orderWorkflowService )
    {
        _orderQueryService =
            orderQueryService;

        _orderWorkflowService =
            orderWorkflowService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(CustomerOrderListResponse) ,
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
    public async Task<ActionResult<CustomerOrderListResponse>>
        GetListAsync(
            [FromQuery] GetCustomerOrdersRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderQueryService.GetCustomerListAsync(
                userId ,
                request ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateQueryFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        if ( result.Response is null )
        {
            return UnexpectedQueryResponseFailure();
        }

        return Ok(result.Response);
    }

    [HttpGet("{orderId:int}")]
    [ProducesResponseType(
        typeof(CustomerOrderDetailsResponse) ,
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
    public async Task<ActionResult<CustomerOrderDetailsResponse>>
        GetDetailsAsync(
            int orderId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderQueryService.GetCustomerDetailsAsync(
                userId ,
                orderId ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateQueryFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        if ( result.Response is null )
        {
            return UnexpectedQueryResponseFailure();
        }

        return Ok(result.Response);
    }

    [HttpPut("{orderId:int}/cancel")]
    [ProducesResponseType(
        typeof(OrderWorkflowResponse) ,
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
    public async Task<ActionResult<OrderWorkflowResponse>>
        CancelAsync(
            int orderId ,
            [FromBody] CancelOrderRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderWorkflowService
                .CancelByCustomerAsync(
                    orderId ,
                    userId ,
                    request.Reason ,
                    cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateWorkflowFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    private ObjectResult InvalidAuthenticationToken()
    {
        var problemDetails = new ProblemDetails
        {
            Status =
                StatusCodes.Status401Unauthorized ,

            Title =
                "Invalid authentication token." ,

            Detail =
                "The authenticated customer identifier is invalid."
        };

        problemDetails.Extensions["code"] =
            OrderQueryErrorCodes.CustomerNotFound;

        return StatusCode(
            StatusCodes.Status401Unauthorized ,
            problemDetails);
    }

    private ObjectResult CreateQueryFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode = errorCode switch
        {
            OrderQueryErrorCodes.InvalidRequest =>
                StatusCodes.Status400BadRequest,

            OrderQueryErrorCodes.CustomerNotFound =>
                StatusCodes.Status401Unauthorized,

            OrderQueryErrorCodes.CustomerBlocked =>
                StatusCodes.Status403Forbidden,

            OrderQueryErrorCodes.OrderNotFound =>
                StatusCodes.Status404NotFound,

            OrderQueryErrorCodes.OrderAccessDenied =>
                StatusCodes.Status403Forbidden,

            OrderQueryErrorCodes.OperationFailed =>
                StatusCodes.Status500InternalServerError,

            _ =>
                StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode ,

            Title =
                "Customer order query failed." ,

            Detail =
                errorMessage ??
                "The customer order request could not be completed."
        };

        problemDetails.Extensions["code"] =
            errorCode ??
            OrderQueryErrorCodes.OperationFailed;

        return StatusCode(
            statusCode ,
            problemDetails);
    }

    private ObjectResult UnexpectedQueryResponseFailure()
    {
        var problemDetails = new ProblemDetails
        {
            Status =
                StatusCodes.Status500InternalServerError ,

            Title =
                "Customer order response failed." ,

            Detail =
                "The order query succeeded, but its response could not be returned."
        };

        problemDetails.Extensions["code"] =
            "orders.response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }

    private ObjectResult CreateWorkflowFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        return errorCode switch
        {
            OrderWorkflowErrorCodes.OrderNotFound =>
                Problem(
                    statusCode:
                        StatusCodes.Status404NotFound ,

                    title:
                        "Order not found." ,

                    detail:
                        errorMessage),

            OrderWorkflowErrorCodes.OrderAccessDenied =>
                Problem(
                    statusCode:
                        StatusCodes.Status403Forbidden ,

                    title:
                        "Order access denied." ,

                    detail:
                        errorMessage),

            OrderWorkflowErrorCodes.InvalidStatusTransition =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Invalid order status transition." ,

                    detail:
                        errorMessage),

            OrderWorkflowErrorCodes.InvalidReason =>
                Problem(
                    statusCode:
                        StatusCodes.Status400BadRequest ,

                    title:
                        "Invalid cancellation reason." ,

                    detail:
                        errorMessage),

            OrderWorkflowErrorCodes.ConcurrencyConflict =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Order data changed." ,

                    detail:
                        errorMessage),

            OrderWorkflowErrorCodes.OperationFailed =>
                Problem(
                    statusCode:
                        StatusCodes.Status500InternalServerError ,

                    title:
                        "Order cancellation failed." ,

                    detail:
                        errorMessage),

            _ =>
                Problem(
                    statusCode:
                        StatusCodes.Status500InternalServerError ,

                    title:
                        "Order cancellation failed." ,

                    detail:
                        "The order could not be cancelled.")
        };
    }
}