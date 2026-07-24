using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.DeliveryAreas.Contracts.Requests;
using Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;
using Mawasem.Application.Features.DeliveryAreas.Interfaces;
using Mawasem.Application.Features.DeliveryAreas.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/delivery-areas")]
public sealed class AdminDeliveryAreasController : ControllerBase
{
    private readonly IDeliveryAreaService _deliveryAreaService;

    public AdminDeliveryAreasController(
        IDeliveryAreaService deliveryAreaService )
    {
        _deliveryAreaService = deliveryAreaService;
    }

    [RequirePermission(SystemPermissions.DeliveryAreas.View)]
    [HttpGet]
    [ProducesResponseType(
        typeof(DeliveryAreaListResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<DeliveryAreaListResponse>>
        GetListAsync(
            [FromQuery] GetDeliveryAreasRequest request ,
            CancellationToken cancellationToken )
    {
        var result =
            await _deliveryAreaService.GetAdminListAsync(
                request ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [RequirePermission(SystemPermissions.DeliveryAreas.View)]
    [HttpGet("{deliveryAreaId:int}")]
    [ProducesResponseType(
        typeof(DeliveryAreaResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<DeliveryAreaResponse>>
        GetByIdAsync(
            int deliveryAreaId ,
            CancellationToken cancellationToken )
    {
        var result = await _deliveryAreaService.GetByIdAsync(
            deliveryAreaId ,
            cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [RequirePermission(SystemPermissions.DeliveryAreas.Create)]
    [HttpPost]
    [ProducesResponseType(
        typeof(DeliveryAreaResponse) ,
        StatusCodes.Status201Created)]
    public async Task<ActionResult<DeliveryAreaResponse>>
        CreateAsync(
            [FromBody] CreateDeliveryAreaRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var actorUserId) )
        {
            return InvalidActorResponse();
        }

        var result = await _deliveryAreaService.CreateAsync(
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
            return UnexpectedResponseFailure();
        }

        return CreatedAtAction(
            nameof(GetByIdAsync) ,
            new
            {
                deliveryAreaId = result.Response.Id
            } ,
            result.Response);
    }

    [RequirePermission(SystemPermissions.DeliveryAreas.Edit)]
    [HttpPut("{deliveryAreaId:int}")]
    [ProducesResponseType(
        typeof(DeliveryAreaResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<DeliveryAreaResponse>>
        UpdateAsync(
            int deliveryAreaId ,
            [FromBody] UpdateDeliveryAreaRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var actorUserId) )
        {
            return InvalidActorResponse();
        }

        var result = await _deliveryAreaService.UpdateAsync(
            actorUserId ,
            deliveryAreaId ,
            request ,
            cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [RequirePermission(SystemPermissions.DeliveryAreas.Edit)]
    [HttpPut("{deliveryAreaId:int}/status")]
    [ProducesResponseType(
        typeof(DeliveryAreaResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<DeliveryAreaResponse>>
        UpdateStatusAsync(
            int deliveryAreaId ,
            [FromBody] UpdateDeliveryAreaStatusRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var actorUserId) )
        {
            return InvalidActorResponse();
        }

        var result =
            await _deliveryAreaService.UpdateStatusAsync(
                actorUserId ,
                deliveryAreaId ,
                request ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [RequirePermission(SystemPermissions.DeliveryAreas.Delete)]
    [HttpDelete("{deliveryAreaId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(
        int deliveryAreaId ,
        CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var actorUserId) )
        {
            return InvalidActorResponse();
        }

        var result = await _deliveryAreaService.DeleteAsync(
            actorUserId ,
            deliveryAreaId ,
            cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return NoContent();
    }

    [RequirePermission(SystemPermissions.DeliveryAreas.Delete)]
    [HttpPost("{deliveryAreaId:int}/restore")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RestoreAsync(
        int deliveryAreaId ,
        CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var actorUserId) )
        {
            return InvalidActorResponse();
        }

        var result = await _deliveryAreaService.RestoreAsync(
            actorUserId ,
            deliveryAreaId ,
            cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return NoContent();
    }

    private ActionResult CreateFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode = errorCode switch
        {
            DeliveryAreaErrorCodes.NotFound =>
                StatusCodes.Status404NotFound,

            DeliveryAreaErrorCodes.DuplicateName =>
                StatusCodes.Status409Conflict,

            DeliveryAreaErrorCodes.HasActiveAddresses =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status400BadRequest
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode ,
            Title = "Delivery-area request failed." ,
            Detail =
                errorMessage ??
                "The delivery-area request could not be completed."
        };

        problemDetails.Extensions["code"] =
            errorCode ??
            DeliveryAreaErrorCodes.InvalidRequest;

        return StatusCode(statusCode , problemDetails);
    }

    private ActionResult InvalidActorResponse()
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized ,
            Title = "Dashboard authentication failed." ,
            Detail =
                "The authenticated dashboard account is invalid."
        };

        problemDetails.Extensions["code"] =
            DeliveryAreaErrorCodes.InvalidRequest;

        return Unauthorized(problemDetails);
    }

    private ActionResult UnexpectedResponseFailure()
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError ,
            Title = "Delivery-area response failed." ,
            Detail =
                "The operation succeeded, but its response could not be returned."
        };

        problemDetails.Extensions["code"] =
            "delivery_areas.response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}