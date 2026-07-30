using Mawasem.API.Extensions;
using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Contracts.Responses;
using Mawasem.Application.Features.Checkout.Interfaces;
using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize(Roles = SystemRoles.Customer)]
public sealed class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;

    public CheckoutController(
        ICheckoutService checkoutService )
    {
        _checkoutService = checkoutService;
    }

    [HttpPost("preview")]
    [ProducesResponseType(
        typeof(CheckoutPreviewResponse) ,
        StatusCodes.Status200OK)]
    public async Task<ActionResult<CheckoutPreviewResponse>>
        PreviewAsync(
            [FromBody] CheckoutPreviewRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _checkoutService.PreviewAsync(
                userId ,
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

    [HttpPost("place-order")]
    [ProducesResponseType(
        typeof(PlaceOrderResponse) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(PlaceOrderResponse) ,
        StatusCodes.Status201Created)]
    public async Task<ActionResult<PlaceOrderResponse>>
        PlaceOrderAsync(
            [FromBody] PlaceOrderRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var userId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _checkoutService.PlaceOrderAsync(
                userId ,
                request ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        if ( result.Response!.IsIdempotentReplay )
        {
            return Ok(result.Response);
        }

        return StatusCode(
            StatusCodes.Status201Created ,
            result.Response);
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
            CheckoutErrorCodes.CustomerNotFound =>
                Problem(
                    statusCode:
                        StatusCodes.Status401Unauthorized ,

                    title:
                        "Invalid customer account." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.CustomerBlocked =>
                Problem(
                    statusCode:
                        StatusCodes.Status403Forbidden ,

                    title:
                        "Customer account blocked." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.CartNotFound =>
                Problem(
                    statusCode:
                        StatusCodes.Status404NotFound ,

                    title:
                        "Customer cart not found." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.CartEmpty =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Customer cart is empty." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.ProductUnavailable =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Product unavailable." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.VariantUnavailable =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Product variant unavailable." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.InsufficientStock =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Insufficient stock." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.AddressNotFound =>
                Problem(
                    statusCode:
                        StatusCodes.Status404NotFound ,

                    title:
                        "Customer address not found." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.AddressNotOwned =>
                Problem(
                    statusCode:
                        StatusCodes.Status403Forbidden ,

                    title:
                        "Customer address access denied." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.AddressInactive =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Customer address inactive." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.DeliveryAreaNotFound =>
                Problem(
                    statusCode:
                        StatusCodes.Status404NotFound ,

                    title:
                        "Delivery area not found." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.DeliveryAreaInactive =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Delivery area inactive." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.DeliveryAreaNotConfirmed =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Delivery area not confirmed." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.PaymentMethodNotSupported =>
                Problem(
                    statusCode:
                        StatusCodes.Status400BadRequest ,

                    title:
                        "Payment method not supported." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.InvalidIdempotencyKey =>
                Problem(
                    statusCode:
                        StatusCodes.Status400BadRequest ,

                    title:
                        "Invalid idempotency key." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.InvalidNotes =>
                Problem(
                    statusCode:
                        StatusCodes.Status400BadRequest ,

                    title:
                        "Invalid order notes." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.ConcurrencyConflict =>
                Problem(
                    statusCode:
                        StatusCodes.Status409Conflict ,

                    title:
                        "Checkout data changed." ,

                    detail:
                        errorMessage),

            CheckoutErrorCodes.OrderCreationFailed =>
                Problem(
                    statusCode:
                        StatusCodes.Status500InternalServerError ,

                    title:
                        "Order creation failed." ,

                    detail:
                        errorMessage),

            _ =>
                Problem(
                    statusCode:
                        StatusCodes.Status500InternalServerError ,

                    title:
                        "Checkout operation failed." ,

                    detail:
                        "The Checkout operation could not be completed.")
        };
    }
}