using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.Reviews.Contracts.Requests;
using Mawasem.Application.Features.Reviews.Contracts.Responses;
using Mawasem.Application.Features.Reviews.Interfaces;
using Mawasem.Application.Features.Reviews.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/reviews")]
public sealed class AdminReviewsController
    : ControllerBase
{
    private readonly IReviewService _reviewService;

    public AdminReviewsController(
        IReviewService reviewService )
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    [RequirePermission(SystemPermissions.Reviews.View)]
    [ProducesResponseType(
        typeof(PagedReviewResponse<AdminReviewResponse>) ,
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
        ActionResult<PagedReviewResponse<AdminReviewResponse>>>
        GetListAsync(
            [FromQuery] GetAdminReviewsRequest request ,
            CancellationToken cancellationToken )
    {
        var result =
            await _reviewService.GetAdminListAsync(
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

    [HttpGet("{reviewId:int}")]
    [RequirePermission(SystemPermissions.Reviews.View)]
    [ProducesResponseType(
        typeof(AdminReviewResponse) ,
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
    public async Task<ActionResult<AdminReviewResponse>>
        GetDetailsAsync(
            int reviewId ,
            CancellationToken cancellationToken )
    {
        var result =
            await _reviewService.GetAdminDetailsAsync(
                reviewId ,
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

    [HttpPost("{reviewId:int}/hide")]
    [RequirePermission(SystemPermissions.Reviews.Moderate)]
    [ProducesResponseType(
        typeof(AdminReviewResponse) ,
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
    public async Task<ActionResult<AdminReviewResponse>>
        HideAsync(
            int reviewId ,
            [FromBody] HideReviewRequest request ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var dashboardUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _reviewService.HideAsync(
                reviewId ,
                dashboardUserId ,
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

    [HttpPost("{reviewId:int}/restore")]
    [RequirePermission(SystemPermissions.Reviews.Moderate)]
    [ProducesResponseType(
        typeof(AdminReviewResponse) ,
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
    public async Task<ActionResult<AdminReviewResponse>>
        RestoreAsync(
            int reviewId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var dashboardUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _reviewService.RestoreAsync(
                reviewId ,
                dashboardUserId ,
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
    [RequirePermission(SystemPermissions.Reviews.Delete)]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
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
    public async Task<IActionResult>
        DeleteAsync(
            int reviewId ,
            CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(
                out var dashboardUserId) )
        {
            return InvalidAuthenticationToken();
        }

        var result =
            await _reviewService.DeleteAdminReviewAsync(
                reviewId ,
                dashboardUserId ,
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
                    "The authenticated dashboard user " +
                    "identifier is invalid."
            };

        problemDetails.Extensions["code"] =
            ReviewErrorCodes.InvalidRequest;

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

                ReviewErrorCodes.CustomerBlocked =>
                    StatusCodes.Status403Forbidden,

                ReviewErrorCodes.ReviewAccessDenied =>
                    StatusCodes.Status403Forbidden,

                ReviewErrorCodes.ProductNotFound =>
                    StatusCodes.Status404NotFound,

                ReviewErrorCodes.CustomerNotFound =>
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
                    "Admin review operation failed." ,

                Detail =
                    errorMessage ??
                    "The admin review operation could not " +
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
                    "Admin review response failed." ,

                Detail =
                    "The review operation succeeded, but its " +
                    "response could not be returned."
            };

        problemDetails.Extensions["code"] =
            "reviews.admin_response_failed";

        return StatusCode(
            StatusCodes.Status500InternalServerError ,
            problemDetails);
    }
}