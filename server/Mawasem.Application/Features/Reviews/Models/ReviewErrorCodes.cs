namespace Mawasem.Application.Features.Reviews.Models;

public static class ReviewErrorCodes
{
    public const string InvalidRequest =
        "reviews.invalid_request";

    public const string ProductNotFound =
        "reviews.product_not_found";

    public const string CustomerNotFound =
        "reviews.customer_not_found";

    public const string CustomerBlocked =
        "reviews.customer_blocked";

    public const string ReviewNotFound =
        "reviews.not_found";

    public const string ReviewAccessDenied =
        "reviews.access_denied";

    public const string InvalidRating =
        "reviews.invalid_rating";

    public const string InvalidComment =
        "reviews.invalid_comment";

    public const string InvalidModerationReason =
        "reviews.invalid_moderation_reason";

    public const string AlreadyHidden =
        "reviews.already_hidden";

    public const string AlreadyVisible =
        "reviews.already_visible";

    public const string OperationFailed =
        "reviews.operation_failed";
}