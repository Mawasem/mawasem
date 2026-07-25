using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.Orders.Contracts.Requests;
using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Interfaces;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
public sealed class AdminOrderWorkflowController
    : ControllerBase
{
    private readonly IOrderWorkflowService
        _orderWorkflowService;

    public AdminOrderWorkflowController(
        IOrderWorkflowService orderWorkflowService )
    {
        _orderWorkflowService =
            orderWorkflowService;
    }

    [HttpPut("{orderId:int}/confirm")]
    [RequirePermission(
        SystemPermissions.Orders.UpdateStatus)]
    [ProducesResponseType(
        typeof(OrderWorkflowResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderWorkflowResponse>>
        ConfirmAsync(
            int orderId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderWorkflowService.ConfirmAsync(
                orderId ,
                userId ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpPut("{orderId:int}/prepare")]
    [RequirePermission(
        SystemPermissions.Orders.UpdateStatus)]
    [ProducesResponseType(
        typeof(OrderWorkflowResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderWorkflowResponse>>
        PrepareAsync(
            int orderId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderWorkflowService.PrepareAsync(
                orderId ,
                userId ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpPut("{orderId:int}/ship")]
    [RequirePermission(
        SystemPermissions.Orders.UpdateStatus)]
    [ProducesResponseType(
        typeof(OrderWorkflowResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderWorkflowResponse>>
        ShipAsync(
            int orderId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderWorkflowService.ShipAsync(
                orderId ,
                userId ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpPut("{orderId:int}/deliver")]
    [RequirePermission(
        SystemPermissions.Orders.UpdateStatus)]
    [ProducesResponseType(
        typeof(OrderWorkflowResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderWorkflowResponse>>
        DeliverAsync(
            int orderId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderWorkflowService.DeliverAsync(
                orderId ,
                userId ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpPut("{orderId:int}/reject")]
    [RequirePermission(
        SystemPermissions.Orders.UpdateStatus)]
    [ProducesResponseType(
        typeof(OrderWorkflowResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderWorkflowResponse>>
        RejectAsync(
            int orderId ,
            [FromBody] RejectOrderRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _orderWorkflowService.RejectAsync(
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

    [HttpPut("{orderId:int}/cancel")]
    [RequirePermission(
        SystemPermissions.Orders.Cancel)]
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
                .CancelByDashboardAsync(
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
                "The authenticated dashboard user identifier is invalid.");
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
                        "Invalid order reason." ,

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
                        "Order workflow operation failed." ,

                    detail:
                        errorMessage),

            _ =>
                Problem(
                    statusCode:
                        StatusCodes.Status500InternalServerError ,

                    title:
                        "Order workflow operation failed." ,

                    detail:
                        "The order operation could not be completed.")
        };
    }
}