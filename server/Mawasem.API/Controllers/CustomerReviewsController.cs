using Mawasem.API.Extensions;
using Mawasem.Application.Features.Reviews.Contracts.Requests;
using Mawasem.Application.Features.Reviews.Contracts.Responses;
using Mawasem.Application.Features.Reviews.Interfaces;
using Mawasem.Application.Features.Reviews.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize(Roles = SystemRoles.Customer)]
public sealed class CustomerReviewsController
    : ControllerBase
{
    private readonly IReviewService _reviewService;

    public CustomerReviewsController(
        IReviewService reviewService )
    {
        _reviewService = reviewService;
    }

    [HttpGet("mine")]
    [ProducesResponseType(
        typeof(PagedReviewResponse<CustomerReviewResponse>) ,
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
        ActionResult<PagedReviewResponse<CustomerReviewResponse>>>
        GetMineAsync(
            [FromQuery] GetReviewsRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var customerUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _reviewService.GetCustomerListAsync(
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

    [HttpPut("{reviewId:int}")]
    [ProducesResponseType(
        typeof(CustomerReviewResponse) ,
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
    public async Task<ActionResult<CustomerReviewResponse>>
        UpdateAsync(
            int reviewId ,
            [FromBody] UpdateReviewRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var customerUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _reviewService.UpdateAsync(
                reviewId ,
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

    [HttpDelete("{reviewId:int}")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
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
    public async Task<IActionResult>
        DeleteAsync(
            int reviewId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var customerUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _reviewService.DeleteCustomerReviewAsync(
                reviewId ,
                customerUserId ,
                cancellationToken);

        if ( !result.Succeeded )
        {
            return CreateFailureResponse(
                result.ErrorCode ,
                result.ErrorMessage);
        }

        return NoContent();
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
                    "The authenticated customer identifier " +
                    "is invalid."
            };

        problemDetails.Extensions["code"] =
            ReviewErrorCodes.CustomerNotFound;

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
                ReviewErrorCodes.InvalidRequest =>
                    StatusCodes.Status400BadRequest,

                ReviewErrorCodes.InvalidRating =>
                    StatusCodes.Status400BadRequest,

                ReviewErrorCodes.InvalidComment =>
                    StatusCodes.Status400BadRequest,

                ReviewErrorCodes.CustomerNotFound =>
                    StatusCodes.Status401Unauthorized,

                ReviewErrorCodes.CustomerBlocked =>
                    StatusCodes.Status403Forbidden,

                ReviewErrorCodes.ReviewAccessDenied =>
                    StatusCodes.Status403Forbidden,

                ReviewErrorCodes.ProductNotFound =>
                    StatusCodes.Status404NotFound,

                ReviewErrorCodes.ReviewNotFound =>
                    StatusCodes.Status404NotFound,

                ReviewErrorCodes.OperationFailed =>
                    StatusCodes.Status500InternalServerError,

                _ =>
                    StatusCodes.Status500InternalServerError
            };

        var problemDetails =
            new ProblemDetails
            {
                Status = statusCode ,

                Title =
                    "Customer review operation failed." ,

                Detail =
                    errorMessage ??
                    "The customer review operation could not " +
                    "be completed."
            };

        problemDetails.Extensions["code"] =
            errorCode ??
            ReviewErrorCodes.OperationFailed;

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
                    "Customer review response failed." ,

                Detail =
                    "The review operation succeeded, but its " +
                    "response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "reviews.customer_response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}