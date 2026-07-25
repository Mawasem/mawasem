using Mawasem.API.Authorization;
using Mawasem.Application.Features.Orders.Contracts.Requests;
using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Interfaces;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
public sealed class AdminOrdersController : ControllerBase
{
    private readonly IOrderQueryService _orderQueryService;

    public AdminOrdersController(
        IOrderQueryService orderQueryService )
    {
        _orderQueryService = orderQueryService;
    }

    [HttpGet]
    [RequirePermission(SystemPermissions.Orders.View)]
    [ProducesResponseType(
        typeof(AdminOrderListResponse) ,
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
    public async Task<ActionResult<AdminOrderListResponse>>
        GetListAsync(
            [FromQuery] GetAdminOrdersRequest request ,
            CancellationToken cancellationToken )
    {
        var result =
            await _orderQueryService.GetAdminListAsync(
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

    [HttpGet("{orderId:int}")]
    [RequirePermission(SystemPermissions.Orders.View)]
    [ProducesResponseType(
        typeof(AdminOrderDetailsResponse) ,
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
    public async Task<ActionResult<AdminOrderDetailsResponse>>
        GetDetailsAsync(
            int orderId ,
            CancellationToken cancellationToken )
    {
        var result =
            await _orderQueryService.GetAdminDetailsAsync(
                orderId ,
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

    private ObjectResult CreateFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode =
            errorCode switch
            {
                OrderQueryErrorCodes.InvalidRequest =>
                    StatusCodes.Status400BadRequest,

                OrderQueryErrorCodes.OrderNotFound =>
                    StatusCodes.Status404NotFound,

                OrderQueryErrorCodes.OperationFailed =>
                    StatusCodes.Status500InternalServerError,

                _ =>
                    StatusCodes.Status500InternalServerError
            };

        var problemDetails =
            new ProblemDetails
            {
                Status = statusCode ,

                Title =
                    "Admin order query failed." ,

                Detail =
                    errorMessage ??
                    "The admin order request could not be completed."
            };

        problemDetails.Extensions["code"] =
            errorCode ??
            OrderQueryErrorCodes.OperationFailed;

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
                    "Admin order response failed." ,

                Detail =
                    "The order query succeeded, but its response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "orders.admin_response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}