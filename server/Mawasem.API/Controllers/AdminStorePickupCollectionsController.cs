using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Contracts.Responses;
using Mawasem.Application.Features.StoreOrders.Interfaces;
using Mawasem.Application.Features.StoreOrders.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/store/orders")]
public sealed class AdminStorePickupCollectionsController
    : ControllerBase
{
    private readonly IStorePickupCollectionService
        _storePickupCollectionService;

    public AdminStorePickupCollectionsController(
        IStorePickupCollectionService
            storePickupCollectionService )
    {
        _storePickupCollectionService =
            storePickupCollectionService;
    }

    [HttpPost("{orderId:int}/collect")]
    [RequirePermission(
        SystemPermissions.Orders.CollectStorePickup)]
    [ProducesResponseType(
        typeof(StorePickupCollectionResponse) ,
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
    public async Task<ActionResult<StorePickupCollectionResponse>>
        CollectAsync(
            int orderId ,
            [FromBody] CollectStorePickupOrderRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var employeeId) )
        {
            return Unauthorized();
        }

        var result =
            await _storePickupCollectionService.CollectAsync(
                orderId ,
                employeeId ,
                request ,
                cancellationToken);

        if ( result.Succeeded &&
            result.Response is not null )
        {
            return Ok(result.Response);
        }

        return CreateFailureResponse(
            result.ErrorCode ,
            result.ErrorMessage);
    }

    private ObjectResult CreateFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode =
            errorCode switch
            {
                StorePickupCollectionErrorCodes.OrderNotFound =>
                    StatusCodes.Status404NotFound,

                StorePickupCollectionErrorCodes.AlreadyCollected =>
                    StatusCodes.Status409Conflict,

                StorePickupCollectionErrorCodes.InvalidOrderStatus =>
                    StatusCodes.Status409Conflict,

                StorePickupCollectionErrorCodes.ConcurrencyConflict =>
                    StatusCodes.Status409Conflict,

                StorePickupCollectionErrorCodes.OperationFailed =>
                    StatusCodes.Status500InternalServerError,

                _ =>
                    StatusCodes.Status400BadRequest
            };

        var problemDetails =
            new ProblemDetails
            {
                Status =
                    statusCode ,

                Title =
                    "Store pickup collection failed." ,

                Detail =
                    errorMessage ??
                    "The store pickup order could not be collected."
            };

        problemDetails.Extensions["code"] =
            errorCode ??
            StorePickupCollectionErrorCodes.OperationFailed;

        return StatusCode(
            statusCode ,
            problemDetails);
    }
}