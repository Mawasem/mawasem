using Mawasem.Application.Features.Reviews.Contracts.Requests;
using Mawasem.Application.Features.Reviews.Contracts.Responses;
using Mawasem.Application.Features.Reviews.Models;

namespace Mawasem.Application.Features.Reviews.Interfaces;

public interface IReviewService
{
    Task<ReviewResult<CustomerReviewResponse>> CreateAsync(
        int productId ,
        int customerUserId ,
        CreateReviewRequest request ,
        CancellationToken cancellationToken = default );

    Task<ReviewResult<CustomerReviewResponse>> UpdateAsync(
        int reviewId ,
        int customerUserId ,
        UpdateReviewRequest request ,
        CancellationToken cancellationToken = default );

    Task<ReviewOperationResult> DeleteCustomerReviewAsync(
        int reviewId ,
        int customerUserId ,
        CancellationToken cancellationToken = default );

    Task<ReviewResult<
        PagedReviewResponse<CustomerReviewResponse>>>
        GetCustomerListAsync(
            int customerUserId ,
            GetReviewsRequest request ,
            CancellationToken cancellationToken = default );

    Task<ReviewResult<
        PagedReviewResponse<PublicReviewResponse>>>
        GetPublicListAsync(
            int productId ,
            GetReviewsRequest request ,
            CancellationToken cancellationToken = default );

    Task<ReviewResult<ReviewSummaryResponse>>
        GetPublicSummaryAsync(
            int productId ,
            CancellationToken cancellationToken = default );

    Task<ReviewResult<
        PagedReviewResponse<AdminReviewResponse>>>
        GetAdminListAsync(
            GetAdminReviewsRequest request ,
            CancellationToken cancellationToken = default );

    Task<ReviewResult<AdminReviewResponse>>
        GetAdminDetailsAsync(
            int reviewId ,
            CancellationToken cancellationToken = default );

    Task<ReviewResult<AdminReviewResponse>> HideAsync(
        int reviewId ,
        int dashboardUserId ,
        HideReviewRequest request ,
        CancellationToken cancellationToken = default );

    Task<ReviewResult<AdminReviewResponse>> RestoreAsync(
        int reviewId ,
        int dashboardUserId ,
        CancellationToken cancellationToken = default );

    Task<ReviewOperationResult> DeleteAdminReviewAsync(
        int reviewId ,
        int dashboardUserId ,
        CancellationToken cancellationToken = default );
}