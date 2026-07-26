using Mawasem.Application.Features.Reviews.Contracts.Requests;
using Mawasem.Application.Features.Reviews.Contracts.Responses;
using Mawasem.Application.Features.Reviews.Interfaces;
using Mawasem.Application.Features.Reviews.Models;
using Mawasem.Domain.Identity;
using Mawasem.Domain.Reviews;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mawasem.Infrastructure.Reviews;

public sealed class ReviewService : IReviewService
{
    private const int MinimumRating = 1;

    private const int MaximumRating = 5;

    private const int MinimumCommentLength = 3;

    private const int MaximumCommentLength = 1000;

    private const int MaximumModerationReasonLength = 500;

    private const int MaximumSearchLength = 256;

    private const int MaximumPageSize = 100;

    private readonly MawasemDbContext _dbContext;

    private readonly TimeProvider _timeProvider;

    public ReviewService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<ReviewResult<CustomerReviewResponse>>
        CreateAsync(
            int productId ,
            int customerUserId ,
            CreateReviewRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( productId <= 0 )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.ProductNotFound ,
                "The product was not found.");
        }

        if ( customerUserId <= 0 )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.CustomerNotFound ,
                "The customer was not found.");
        }

        var validationFailure =
            ValidateReviewContent(
                request.Rating ,
                request.Comment ,
                out var normalizedComment);

        if ( validationFailure is not null )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                validationFailure.Value.ErrorCode ,
                validationFailure.Value.ErrorMessage);
        }

        var customerAccessState =
            await GetCustomerAccessStateAsync(
                customerUserId ,
                cancellationToken);

        var customerAccessFailure =
            CreateCustomerAccessFailure<CustomerReviewResponse>(
                customerAccessState);

        if ( customerAccessFailure is not null )
        {
            return customerAccessFailure;
        }

        var productExists =
            await _dbContext.Products
                .AsNoTracking()
                .AnyAsync(
                    product =>
                        product.Id == productId &&
                        !product.IsDeleted &&
                        product.IsPublished ,
                    cancellationToken);

        if ( !productExists )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.ProductNotFound ,
                "The product was not found.");
        }

        var now =
            _timeProvider.GetUtcNow();

        var actor =
            customerUserId.ToString(
                CultureInfo.InvariantCulture);

        var review =
            new Review
            {
                ProductId = productId ,
                UserId = customerUserId ,
                Rating = request.Rating ,
                Comment = normalizedComment ,
                IsVisible = true ,
                CreatedOn = now ,
                CreatedBy = actor ,
                IsDeleted = false
            };

        _dbContext.Reviews.Add(review);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        var response =
            await GetCustomerResponseByIdAsync(
                review.Id ,
                customerUserId ,
                cancellationToken);

        if ( response is null )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.OperationFailed ,
                "The review was created, but its response could not be returned.");
        }

        return ReviewResult<CustomerReviewResponse>.Success(
            response);
    }

    public async Task<ReviewResult<CustomerReviewResponse>>
        UpdateAsync(
            int reviewId ,
            int customerUserId ,
            UpdateReviewRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( reviewId <= 0 )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( customerUserId <= 0 )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.CustomerNotFound ,
                "The customer was not found.");
        }

        var validationFailure =
            ValidateReviewContent(
                request.Rating ,
                request.Comment ,
                out var normalizedComment);

        if ( validationFailure is not null )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                validationFailure.Value.ErrorCode ,
                validationFailure.Value.ErrorMessage);
        }

        var customerAccessState =
            await GetCustomerAccessStateAsync(
                customerUserId ,
                cancellationToken);

        var customerAccessFailure =
            CreateCustomerAccessFailure<CustomerReviewResponse>(
                customerAccessState);

        if ( customerAccessFailure is not null )
        {
            return customerAccessFailure;
        }

        var review =
            await _dbContext.Reviews
                .AsTracking()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == reviewId ,
                    cancellationToken);

        if ( review is null )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( review.UserId != customerUserId )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.ReviewAccessDenied ,
                "The customer cannot modify this review.");
        }

        var now =
            _timeProvider.GetUtcNow();

        var actor =
            customerUserId.ToString(
                CultureInfo.InvariantCulture);

        review.Rating = request.Rating;
        review.Comment = normalizedComment;
        review.LastModifiedOn = now;
        review.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        var response =
            await GetCustomerResponseByIdAsync(
                review.Id ,
                customerUserId ,
                cancellationToken);

        if ( response is null )
        {
            return ReviewResult<CustomerReviewResponse>.Failure(
                ReviewErrorCodes.OperationFailed ,
                "The review was updated, but its response could not be returned.");
        }

        return ReviewResult<CustomerReviewResponse>.Success(
            response);
    }

    public async Task<ReviewOperationResult>
        DeleteCustomerReviewAsync(
            int reviewId ,
            int customerUserId ,
            CancellationToken cancellationToken = default )
    {
        if ( reviewId <= 0 )
        {
            return ReviewOperationResult.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( customerUserId <= 0 )
        {
            return ReviewOperationResult.Failure(
                ReviewErrorCodes.CustomerNotFound ,
                "The customer was not found.");
        }

        var customerAccessState =
            await GetCustomerAccessStateAsync(
                customerUserId ,
                cancellationToken);

        var customerAccessFailure =
            CreateCustomerOperationAccessFailure(
                customerAccessState);

        if ( customerAccessFailure is not null )
        {
            return customerAccessFailure;
        }

        var review =
            await _dbContext.Reviews
                .AsTracking()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == reviewId ,
                    cancellationToken);

        if ( review is null )
        {
            return ReviewOperationResult.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( review.UserId != customerUserId )
        {
            return ReviewOperationResult.Failure(
                ReviewErrorCodes.ReviewAccessDenied ,
                "The customer cannot delete this review.");
        }

        var now =
            _timeProvider.GetUtcNow();

        var actor =
            customerUserId.ToString(
                CultureInfo.InvariantCulture);

        review.IsDeleted = true;
        review.DeletedOn = now;
        review.DeletedBy = actor;
        review.LastModifiedOn = now;
        review.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return ReviewOperationResult.Success();
    }

    public async Task<ReviewResult<
        PagedReviewResponse<CustomerReviewResponse>>>
        GetCustomerListAsync(
            int customerUserId ,
            GetReviewsRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( customerUserId <= 0 )
        {
            return ReviewResult<
                PagedReviewResponse<CustomerReviewResponse>>
                .Failure(
                    ReviewErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        var paginationFailure =
            ValidatePagination(
                request.PageNumber ,
                request.PageSize ,
                out var skipCount);

        if ( paginationFailure is not null )
        {
            return ReviewResult<
                PagedReviewResponse<CustomerReviewResponse>>
                .Failure(
                    ReviewErrorCodes.InvalidRequest ,
                    paginationFailure);
        }

        var customerAccessState =
            await GetCustomerAccessStateAsync(
                customerUserId ,
                cancellationToken);

        var customerAccessFailure =
            CreateCustomerAccessFailure<
                PagedReviewResponse<CustomerReviewResponse>>(
                    customerAccessState);

        if ( customerAccessFailure is not null )
        {
            return customerAccessFailure;
        }

        var query =
            _dbContext.Reviews
                .AsNoTracking()
                .Where(review =>
                    review.UserId == customerUserId);

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(review =>
                    review.Id)
                .Skip(skipCount)
                .Take(request.PageSize)
                .Select(review =>
                    new CustomerReviewResponse
                    {
                        Id = review.Id ,
                        ProductId = review.ProductId ,
                        ProductNameAr =
                            review.Product.Name.Arabic ,
                        ProductNameEn =
                            review.Product.Name.English ,
                        Rating = review.Rating ,
                        Comment = review.Comment ,
                        IsVisible = review.IsVisible ,
                        ModerationReason =
                            review.ModerationReason ,
                        ModeratedAtUtc =
                            review.ModeratedAtUtc ,
                        CreatedOn = review.CreatedOn ,
                        LastModifiedOn =
                            review.LastModifiedOn
                    })
                .ToArrayAsync(
                    cancellationToken);

        var response =
            new PagedReviewResponse<CustomerReviewResponse>
            {
                Items = items ,
                PageNumber = request.PageNumber ,
                PageSize = request.PageSize ,
                TotalCount = totalCount ,
                TotalPages =
                    CalculateTotalPages(
                        totalCount ,
                        request.PageSize)
            };

        return ReviewResult<
            PagedReviewResponse<CustomerReviewResponse>>
            .Success(response);
    }

    public async Task<ReviewResult<
        PagedReviewResponse<PublicReviewResponse>>>
        GetPublicListAsync(
            int productId ,
            GetReviewsRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( productId <= 0 )
        {
            return ReviewResult<
                PagedReviewResponse<PublicReviewResponse>>
                .Failure(
                    ReviewErrorCodes.ProductNotFound ,
                    "The product was not found.");
        }

        var paginationFailure =
            ValidatePagination(
                request.PageNumber ,
                request.PageSize ,
                out var skipCount);

        if ( paginationFailure is not null )
        {
            return ReviewResult<
                PagedReviewResponse<PublicReviewResponse>>
                .Failure(
                    ReviewErrorCodes.InvalidRequest ,
                    paginationFailure);
        }

        var productExists =
            await PublicProductExistsAsync(
                productId ,
                cancellationToken);

        if ( !productExists )
        {
            return ReviewResult<
                PagedReviewResponse<PublicReviewResponse>>
                .Failure(
                    ReviewErrorCodes.ProductNotFound ,
                    "The product was not found.");
        }

        var query =
            _dbContext.Reviews
                .AsNoTracking()
                .Where(review =>
                    review.ProductId == productId &&
                    review.IsVisible);

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(review =>
                    review.Id)
                .Skip(skipCount)
                .Take(request.PageSize)
                .Select(review =>
                    new PublicReviewResponse
                    {
                        Id = review.Id ,
                        ProductId = review.ProductId ,
                        CustomerDisplayName =
                            review.User.FullNameEn !=
                            string.Empty
                                ? review.User.FullNameEn
                                : review.User.FullNameAr ,
                        Rating = review.Rating ,
                        Comment = review.Comment ,
                        CreatedOn = review.CreatedOn ,
                        LastModifiedOn =
                            review.LastModifiedOn
                    })
                .ToArrayAsync(
                    cancellationToken);

        var response =
            new PagedReviewResponse<PublicReviewResponse>
            {
                Items = items ,
                PageNumber = request.PageNumber ,
                PageSize = request.PageSize ,
                TotalCount = totalCount ,
                TotalPages =
                    CalculateTotalPages(
                        totalCount ,
                        request.PageSize)
            };

        return ReviewResult<
            PagedReviewResponse<PublicReviewResponse>>
            .Success(response);
    }

    public async Task<ReviewResult<ReviewSummaryResponse>>
        GetPublicSummaryAsync(
            int productId ,
            CancellationToken cancellationToken = default )
    {
        if ( productId <= 0 )
        {
            return ReviewResult<ReviewSummaryResponse>.Failure(
                ReviewErrorCodes.ProductNotFound ,
                "The product was not found.");
        }

        var productExists =
            await PublicProductExistsAsync(
                productId ,
                cancellationToken);

        if ( !productExists )
        {
            return ReviewResult<ReviewSummaryResponse>.Failure(
                ReviewErrorCodes.ProductNotFound ,
                "The product was not found.");
        }

        var groupedCounts =
            await _dbContext.Reviews
                .AsNoTracking()
                .Where(review =>
                    review.ProductId == productId &&
                    review.IsVisible)
                .GroupBy(review =>
                    review.Rating)
                .Select(group =>
                    new
                    {
                        Rating = group.Key ,
                        Count = group.Count()
                    })
                .ToArrayAsync(
                    cancellationToken);

        var countsByRating =
            groupedCounts.ToDictionary(
                item => item.Rating ,
                item => item.Count);

        var distribution =
            Enumerable.Range(
                    MinimumRating ,
                    MaximumRating)
                .Select(rating =>
                    new ReviewRatingCountResponse
                    {
                        Rating = rating ,
                        Count =
                            countsByRating.GetValueOrDefault(
                                rating)
                    })
                .ToArray();

        var totalCount =
            distribution.Sum(item =>
                item.Count);

        var weightedRatingTotal =
            distribution.Sum(item =>
                item.Rating * item.Count);

        var averageRating =
            totalCount == 0
                ? 0m
                : Math.Round(
                    weightedRatingTotal /
                    (decimal)totalCount ,
                    2 ,
                    MidpointRounding.AwayFromZero);

        var response =
            new ReviewSummaryResponse
            {
                AverageRating = averageRating ,
                TotalCount = totalCount ,
                Distribution = distribution
            };

        return ReviewResult<ReviewSummaryResponse>.Success(
            response);
    }

    public async Task<ReviewResult<
        PagedReviewResponse<AdminReviewResponse>>>
        GetAdminListAsync(
            GetAdminReviewsRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationFailure =
            ValidateAdminListRequest(
                request ,
                out var skipCount ,
                out var normalizedSearch);

        if ( validationFailure is not null )
        {
            return ReviewResult<
                PagedReviewResponse<AdminReviewResponse>>
                .Failure(
                    ReviewErrorCodes.InvalidRequest ,
                    validationFailure);
        }

        IQueryable<Review> query =
            _dbContext.Reviews
                .AsNoTracking();

        if ( !string.IsNullOrWhiteSpace(
                normalizedSearch) )
        {
            query =
                query.Where(review =>
                    review.Comment.Contains(
                        normalizedSearch) ||
                    review.Product.Name.English.Contains(
                        normalizedSearch) ||
                    review.Product.Name.Arabic.Contains(
                        normalizedSearch) ||
                    review.User.FullNameEn.Contains(
                        normalizedSearch) ||
                    review.User.FullNameAr.Contains(
                        normalizedSearch) ||
                    ( review.User.Email != null &&
                     review.User.Email.Contains(
                         normalizedSearch) ));
        }

        if ( request.ProductId.HasValue )
        {
            query =
                query.Where(review =>
                    review.ProductId ==
                    request.ProductId.Value);
        }

        if ( request.CustomerUserId.HasValue )
        {
            query =
                query.Where(review =>
                    review.UserId ==
                    request.CustomerUserId.Value);
        }

        if ( request.IsVisible.HasValue )
        {
            query =
                query.Where(review =>
                    review.IsVisible ==
                    request.IsVisible.Value);
        }

        if ( request.FromDateUtc.HasValue )
        {
            query =
                query.Where(review =>
                    review.CreatedOn >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            query =
                query.Where(review =>
                    review.CreatedOn <=
                    request.ToDateUtc.Value);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(review =>
                    review.Id)
                .Skip(skipCount)
                .Take(request.PageSize)
                .Select(review =>
                    new AdminReviewResponse
                    {
                        Id = review.Id ,
                        ProductId = review.ProductId ,
                        ProductNameAr =
                            review.Product.Name.Arabic ,
                        ProductNameEn =
                            review.Product.Name.English ,
                        CustomerUserId =
                            review.UserId ,
                        CustomerNameAr =
                            review.User.FullNameAr ,
                        CustomerNameEn =
                            review.User.FullNameEn ,
                        CustomerEmail =
                            review.User.Email ??
                            string.Empty ,
                        Rating = review.Rating ,
                        Comment = review.Comment ,
                        IsVisible = review.IsVisible ,
                        ModerationReason =
                            review.ModerationReason ,
                        ModeratedAtUtc =
                            review.ModeratedAtUtc ,
                        ModeratedByEmployeeId =
                            review.ModeratedByEmployeeId ,
                        CreatedOn = review.CreatedOn ,
                        LastModifiedOn =
                            review.LastModifiedOn
                    })
                .ToArrayAsync(
                    cancellationToken);

        var response =
            new PagedReviewResponse<AdminReviewResponse>
            {
                Items = items ,
                PageNumber = request.PageNumber ,
                PageSize = request.PageSize ,
                TotalCount = totalCount ,
                TotalPages =
                    CalculateTotalPages(
                        totalCount ,
                        request.PageSize)
            };

        return ReviewResult<
            PagedReviewResponse<AdminReviewResponse>>
            .Success(response);
    }

    public async Task<ReviewResult<AdminReviewResponse>>
        GetAdminDetailsAsync(
            int reviewId ,
            CancellationToken cancellationToken = default )
    {
        if ( reviewId <= 0 )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        var response =
            await GetAdminResponseByIdAsync(
                reviewId ,
                cancellationToken);

        if ( response is null )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        return ReviewResult<AdminReviewResponse>.Success(
            response);
    }

    public async Task<ReviewResult<AdminReviewResponse>>
        HideAsync(
            int reviewId ,
            int dashboardUserId ,
            HideReviewRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( reviewId <= 0 )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( !await DashboardActorExistsAsync(
                dashboardUserId ,
                cancellationToken) )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.InvalidRequest ,
                "The dashboard user was not found.");
        }

        var moderationReason =
            request.ModerationReason?.Trim();

        if ( string.IsNullOrWhiteSpace(
                moderationReason) )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.InvalidModerationReason ,
                "A moderation reason is required.");
        }

        if ( moderationReason.Length >
            MaximumModerationReasonLength )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.InvalidModerationReason ,
                $"The moderation reason cannot exceed " +
                $"{MaximumModerationReasonLength} characters.");
        }

        var review =
            await _dbContext.Reviews
                .AsTracking()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == reviewId ,
                    cancellationToken);

        if ( review is null )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( !review.IsVisible )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.AlreadyHidden ,
                "The review is already hidden.");
        }

        var now =
            _timeProvider.GetUtcNow();

        var actor =
            dashboardUserId.ToString(
                CultureInfo.InvariantCulture);

        review.IsVisible = false;
        review.ModerationReason = moderationReason;
        review.ModeratedAtUtc = now;
        review.ModeratedByEmployeeId =
            dashboardUserId;
        review.LastModifiedOn = now;
        review.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        var response =
            await GetAdminResponseByIdAsync(
                review.Id ,
                cancellationToken);

        if ( response is null )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.OperationFailed ,
                "The review was hidden, but its response could not be returned.");
        }

        return ReviewResult<AdminReviewResponse>.Success(
            response);
    }

    public async Task<ReviewResult<AdminReviewResponse>>
        RestoreAsync(
            int reviewId ,
            int dashboardUserId ,
            CancellationToken cancellationToken = default )
    {
        if ( reviewId <= 0 )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( !await DashboardActorExistsAsync(
                dashboardUserId ,
                cancellationToken) )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.InvalidRequest ,
                "The dashboard user was not found.");
        }

        var review =
            await _dbContext.Reviews
                .AsTracking()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == reviewId ,
                    cancellationToken);

        if ( review is null )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( review.IsVisible )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.AlreadyVisible ,
                "The review is already visible.");
        }

        var now =
            _timeProvider.GetUtcNow();

        var actor =
            dashboardUserId.ToString(
                CultureInfo.InvariantCulture);

        review.IsVisible = true;
        review.ModerationReason = null;
        review.ModeratedAtUtc = now;
        review.ModeratedByEmployeeId =
            dashboardUserId;
        review.LastModifiedOn = now;
        review.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        var response =
            await GetAdminResponseByIdAsync(
                review.Id ,
                cancellationToken);

        if ( response is null )
        {
            return ReviewResult<AdminReviewResponse>.Failure(
                ReviewErrorCodes.OperationFailed ,
                "The review was restored, but its response could not be returned.");
        }

        return ReviewResult<AdminReviewResponse>.Success(
            response);
    }

    public async Task<ReviewOperationResult>
        DeleteAdminReviewAsync(
            int reviewId ,
            int dashboardUserId ,
            CancellationToken cancellationToken = default )
    {
        if ( reviewId <= 0 )
        {
            return ReviewOperationResult.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        if ( !await DashboardActorExistsAsync(
                dashboardUserId ,
                cancellationToken) )
        {
            return ReviewOperationResult.Failure(
                ReviewErrorCodes.InvalidRequest ,
                "The dashboard user was not found.");
        }

        var review =
            await _dbContext.Reviews
                .AsTracking()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == reviewId ,
                    cancellationToken);

        if ( review is null )
        {
            return ReviewOperationResult.Failure(
                ReviewErrorCodes.ReviewNotFound ,
                "The review was not found.");
        }

        var now =
            _timeProvider.GetUtcNow();

        var actor =
            dashboardUserId.ToString(
                CultureInfo.InvariantCulture);

        review.IsVisible = false;
        review.IsDeleted = true;
        review.DeletedOn = now;
        review.DeletedBy = actor;
        review.LastModifiedOn = now;
        review.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        return ReviewOperationResult.Success();
    }

    private async Task<CustomerReviewResponse?>
        GetCustomerResponseByIdAsync(
            int reviewId ,
            int customerUserId ,
            CancellationToken cancellationToken )
    {
        return await _dbContext.Reviews
            .AsNoTracking()
            .Where(review =>
                review.Id == reviewId &&
                review.UserId == customerUserId)
            .Select(review =>
                new CustomerReviewResponse
                {
                    Id = review.Id ,
                    ProductId = review.ProductId ,
                    ProductNameAr =
                        review.Product.Name.Arabic ,
                    ProductNameEn =
                        review.Product.Name.English ,
                    Rating = review.Rating ,
                    Comment = review.Comment ,
                    IsVisible = review.IsVisible ,
                    ModerationReason =
                        review.ModerationReason ,
                    ModeratedAtUtc =
                        review.ModeratedAtUtc ,
                    CreatedOn = review.CreatedOn ,
                    LastModifiedOn =
                        review.LastModifiedOn
                })
            .SingleOrDefaultAsync(
                cancellationToken);
    }

    private async Task<AdminReviewResponse?>
        GetAdminResponseByIdAsync(
            int reviewId ,
            CancellationToken cancellationToken )
    {
        return await _dbContext.Reviews
            .AsNoTracking()
            .Where(review =>
                review.Id == reviewId)
            .Select(review =>
                new AdminReviewResponse
                {
                    Id = review.Id ,
                    ProductId = review.ProductId ,
                    ProductNameAr =
                        review.Product.Name.Arabic ,
                    ProductNameEn =
                        review.Product.Name.English ,
                    CustomerUserId =
                        review.UserId ,
                    CustomerNameAr =
                        review.User.FullNameAr ,
                    CustomerNameEn =
                        review.User.FullNameEn ,
                    CustomerEmail =
                        review.User.Email ??
                        string.Empty ,
                    Rating = review.Rating ,
                    Comment = review.Comment ,
                    IsVisible = review.IsVisible ,
                    ModerationReason =
                        review.ModerationReason ,
                    ModeratedAtUtc =
                        review.ModeratedAtUtc ,
                    ModeratedByEmployeeId =
                        review.ModeratedByEmployeeId ,
                    CreatedOn = review.CreatedOn ,
                    LastModifiedOn =
                        review.LastModifiedOn
                })
            .SingleOrDefaultAsync(
                cancellationToken);
    }

    private async Task<CustomerAccessState>
        GetCustomerAccessStateAsync(
            int customerUserId ,
            CancellationToken cancellationToken )
    {
        var isBlocked =
            await (
                from user
                    in _dbContext.Users.AsNoTracking()
                join userRole
                    in _dbContext.UserRoles.AsNoTracking()
                    on user.Id equals userRole.UserId
                join role
                    in _dbContext.Roles.AsNoTracking()
                    on userRole.RoleId equals role.Id
                where
                    user.Id == customerUserId &&
                    role.Name == SystemRoles.Customer
                select (bool?)user.IsBlocked
            ).SingleOrDefaultAsync(
                cancellationToken);

        if ( !isBlocked.HasValue )
        {
            return CustomerAccessState.NotFound;
        }

        return isBlocked.Value
            ? CustomerAccessState.Blocked
            : CustomerAccessState.Allowed;
    }

    private async Task<bool>
        DashboardActorExistsAsync(
            int dashboardUserId ,
            CancellationToken cancellationToken )
    {
        if ( dashboardUserId <= 0 )
        {
            return false;
        }

        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(
                user =>
                    user.Id == dashboardUserId &&
                    !user.IsBlocked ,
                cancellationToken);
    }

    private async Task<bool>
        PublicProductExistsAsync(
            int productId ,
            CancellationToken cancellationToken )
    {
        return await _dbContext.Products
            .AsNoTracking()
            .AnyAsync(
                product =>
                    product.Id == productId &&
                    !product.IsDeleted &&
                    product.IsPublished ,
                cancellationToken);
    }

    private static ReviewResult<TResponse>?
        CreateCustomerAccessFailure<TResponse>(
            CustomerAccessState customerAccessState )
    {
        return customerAccessState switch
        {
            CustomerAccessState.NotFound =>
                ReviewResult<TResponse>.Failure(
                    ReviewErrorCodes.CustomerNotFound ,
                    "The customer was not found."),

            CustomerAccessState.Blocked =>
                ReviewResult<TResponse>.Failure(
                    ReviewErrorCodes.CustomerBlocked ,
                    "The customer account is blocked."),

            _ =>
                null
        };
    }

    private static ReviewOperationResult?
        CreateCustomerOperationAccessFailure(
            CustomerAccessState customerAccessState )
    {
        return customerAccessState switch
        {
            CustomerAccessState.NotFound =>
                ReviewOperationResult.Failure(
                    ReviewErrorCodes.CustomerNotFound ,
                    "The customer was not found."),

            CustomerAccessState.Blocked =>
                ReviewOperationResult.Failure(
                    ReviewErrorCodes.CustomerBlocked ,
                    "The customer account is blocked."),

            _ =>
                null
        };
    }

    private static (
        string ErrorCode ,
        string ErrorMessage)?
        ValidateReviewContent(
            int rating ,
            string? comment ,
            out string normalizedComment )
    {
        normalizedComment =
            comment?.Trim() ??
            string.Empty;

        if ( rating < MinimumRating ||
            rating > MaximumRating )
        {
            return (
                ReviewErrorCodes.InvalidRating ,
                $"Rating must be between " +
                $"{MinimumRating} and {MaximumRating}.");
        }

        if ( normalizedComment.Length <
            MinimumCommentLength )
        {
            return (
                ReviewErrorCodes.InvalidComment ,
                $"The comment must contain at least " +
                $"{MinimumCommentLength} characters.");
        }

        if ( normalizedComment.Length >
            MaximumCommentLength )
        {
            return (
                ReviewErrorCodes.InvalidComment ,
                $"The comment cannot exceed " +
                $"{MaximumCommentLength} characters.");
        }

        return null;
    }

    private static string?
        ValidatePagination(
            int pageNumber ,
            int pageSize ,
            out int skipCount )
    {
        skipCount = 0;

        if ( pageNumber <= 0 )
        {
            return
                "Page number must be greater than zero.";
        }

        if ( pageSize <= 0 ||
            pageSize > MaximumPageSize )
        {
            return
                $"Page size must be between 1 and " +
                $"{MaximumPageSize}.";
        }

        var calculatedSkipCount =
            (long)( pageNumber - 1 ) *
            pageSize;

        if ( calculatedSkipCount > int.MaxValue )
        {
            return
                "The requested page is outside the supported range.";
        }

        skipCount =
            (int)calculatedSkipCount;

        return null;
    }

    private static string?
        ValidateAdminListRequest(
            GetAdminReviewsRequest request ,
            out int skipCount ,
            out string? normalizedSearch )
    {
        normalizedSearch =
            request.Search?.Trim();

        var paginationFailure =
            ValidatePagination(
                request.PageNumber ,
                request.PageSize ,
                out skipCount);

        if ( paginationFailure is not null )
        {
            return paginationFailure;
        }

        if ( normalizedSearch?.Length >
            MaximumSearchLength )
        {
            return
                $"Search text cannot exceed " +
                $"{MaximumSearchLength} characters.";
        }

        if ( request.ProductId.HasValue &&
            request.ProductId.Value <= 0 )
        {
            return
                "The product identifier must be greater than zero.";
        }

        if ( request.CustomerUserId.HasValue &&
            request.CustomerUserId.Value <= 0 )
        {
            return
                "The customer identifier must be greater than zero.";
        }

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
            request.ToDateUtc.Value )
        {
            return
                "The start date cannot be later than the end date.";
        }

        return null;
    }

    private static int CalculateTotalPages(
        int totalCount ,
        int pageSize )
    {
        return totalCount == 0
            ? 0
            : (int)Math.Ceiling(
                totalCount /
                (double)pageSize);
    }

    private enum CustomerAccessState
    {
        Allowed = 1,
        NotFound = 2,
        Blocked = 3
    }
}