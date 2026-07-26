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
[Route("api/products/{productId:int}/reviews")]
public sealed class ProductReviewsController
    : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ProductReviewsController(
        IReviewService reviewService )
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(PagedReviewResponse<PublicReviewResponse>) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status500InternalServerError)]
    public async Task<
        ActionResult<PagedReviewResponse<PublicReviewResponse>>>
        GetListAsync(
            int productId ,
            [FromQuery] GetReviewsRequest request ,
            CancellationToken cancellationToken )
    {
        var result =
            await _reviewService.GetPublicListAsync(
                productId ,
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

    [HttpGet("summary")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ReviewSummaryResponse) ,
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails) ,
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewSummaryResponse>>
        GetSummaryAsync(
            int productId ,
            CancellationToken cancellationToken )
    {
        var result =
            await _reviewService.GetPublicSummaryAsync(
                productId ,
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

    [HttpPost]
    [Authorize(Roles = SystemRoles.Customer)]
    [ProducesResponseType(
        typeof(CustomerReviewResponse) ,
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
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerReviewResponse>>
        CreateAsync(
            int productId ,
            [FromBody] CreateReviewRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var customerUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _reviewService.CreateAsync(
                productId ,
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

                ReviewErrorCodes
                    .InvalidModerationReason =>
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

                ReviewErrorCodes.AlreadyHidden =>
                    StatusCodes.Status409Conflict,

                ReviewErrorCodes.AlreadyVisible =>
                    StatusCodes.Status409Conflict,

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
                    "Review operation failed." ,

                Detail =
                    errorMessage ??
                    "The review operation could not be completed."
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
                    "Review response failed." ,

                Detail =
                    "The review operation succeeded, but its " +
                    "response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "reviews.response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}