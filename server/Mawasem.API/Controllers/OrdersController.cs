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
    private readonly IOrderWorkflowService
        _orderWorkflowService;

    public OrdersController(
        IOrderWorkflowService orderWorkflowService )
    {
        _orderWorkflowService =
            orderWorkflowService;
    }

    [HttpPut("{orderId:int}/cancel")]
    [ProducesResponseType(
        typeof(OrderWorkflowResponse) ,
        StatusCodes.Status200OK)]
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
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    private ObjectResult InvalidAuthenticationToken()
    {
        return Problem(
            statusCode:
                StatusCodes.Status401Unauthorized ,

            title:
                "Invalid authentication token." ,

            detail:
                "The authenticated customer identifier is invalid.");
    }

    private ObjectResult CreateFailureResponse(
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