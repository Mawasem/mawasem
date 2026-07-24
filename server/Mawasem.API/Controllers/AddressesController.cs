using Mawasem.API.Extensions;
using Mawasem.Application.Features.Addresses.Contracts.Requests;
using Mawasem.Application.Features.Addresses.Contracts.Responses;
using Mawasem.Application.Features.Addresses.Interfaces;
using Mawasem.Application.Features.Addresses.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/addresses")]
[Authorize(Roles = SystemRoles.Customer)]
public sealed class AddressesController : ControllerBase
{
    private readonly IUserAddressService _userAddressService;

    public AddressesController(
        IUserAddressService userAddressService )
    {
        _userAddressService = userAddressService;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(UserAddressListResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<UserAddressListResponse>>
        GetListAsync(
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result = await _userAddressService.GetListAsync(
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

    [HttpGet("{addressId:int}")]
    [ProducesResponseType(
        typeof(UserAddressResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<UserAddressResponse>>
        GetByIdAsync(
            int addressId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result = await _userAddressService.GetByIdAsync(
            userId ,
            addressId ,
            cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(UserAddressResponse) ,
        StatusCodes.Status201Created)]
    public async Task<ActionResult<UserAddressResponse>>
        CreateAsync(
            [FromBody] CreateUserAddressRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result = await _userAddressService.CreateAsync(
            userId ,
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
                addressId = result.Response.Id
            } ,
            result.Response);
    }

    [HttpPut("{addressId:int}")]
    [ProducesResponseType(
        typeof(UserAddressResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<UserAddressResponse>>
        UpdateAsync(
            int addressId ,
            [FromBody] UpdateUserAddressRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result = await _userAddressService.UpdateAsync(
            userId ,
            addressId ,
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

    [HttpPut("{addressId:int}/default")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetDefaultAsync(
        int addressId ,
        CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result = await _userAddressService.SetDefaultAsync(
            userId ,
            addressId ,
            cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpDelete("{addressId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAsync(
        int addressId ,
        CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result = await _userAddressService.DeleteAsync(
            userId ,
            addressId ,
            cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return NoContent();
    }

    private ActionResult InvalidAuthenticationToken()
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized ,
            Title = "Customer authentication failed." ,
            Detail =
                "The authenticated customer identifier is invalid."
        };

        problemDetails.Extensions["code"] =
            UserAddressErrorCodes.InvalidCustomer;

        return Unauthorized(problemDetails);
    }

    private ActionResult CreateFailureResponse(
        string? errorCode ,
        string? errorMessage )
    {
        var statusCode = errorCode switch
        {
            UserAddressErrorCodes.InvalidCustomer =>
                StatusCodes.Status401Unauthorized,

            UserAddressErrorCodes.AccountBlocked =>
                StatusCodes.Status403Forbidden,

            UserAddressErrorCodes.AddressNotFound =>
                StatusCodes.Status404NotFound,

            UserAddressErrorCodes.DeliveryAreaNotFound =>
                StatusCodes.Status404NotFound,

            UserAddressErrorCodes.DeliveryAreaUnavailable =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status400BadRequest
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode ,
            Title = "Customer address request failed." ,
            Detail =
                errorMessage ??
                "The customer address request could not be completed."
        };

        problemDetails.Extensions["code"] =
            errorCode ??
            UserAddressErrorCodes.InvalidRequest;

        return StatusCode(statusCode , problemDetails);
    }

    private ActionResult UnexpectedResponseFailure()
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError ,
            Title = "Customer address response failed." ,
            Detail =
                "The address operation succeeded, but its response could not be returned."
        };

        problemDetails.Extensions["code"] =
            "addresses.response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}