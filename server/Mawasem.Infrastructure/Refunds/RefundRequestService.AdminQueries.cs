using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Application.Features.Refunds.Models;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Refunds;

public sealed partial class RefundRequestService
{
    public async Task<
        RefundRequestResult<AdminRefundRequestListResponse>>
        GetAdminListAsync(
            GetAdminRefundRequestsRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationError =
            ValidateAdminListRequest(request);

        if ( validationError is not null )
        {
            return RefundRequestResult<
                AdminRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    validationError);
        }

        var skipCount =
            (long)( request.PageNumber - 1 ) *
            request.PageSize;

        if ( skipCount > int.MaxValue )
        {
            return RefundRequestResult<
                AdminRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The requested page is outside the " +
                    "supported range.");
        }

        var search =
            request.Search?.Trim();

        var query =
            _dbContext.RefundRequests
                .AsNoTracking()
                .Where(refundRequest =>
                    !refundRequest.IsDeleted &&
                    !refundRequest.Order.IsDeleted);

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            query =
                query.Where(refundRequest =>
                    refundRequest.Order.OrderNumber
                        .Contains(search) ||
                    refundRequest.Order.CustomerNameAr
                        .Contains(search) ||
                    refundRequest.Order.CustomerNameEn
                        .Contains(search) ||
                    refundRequest.Order.CustomerPhone
                        .Contains(search) ||
                    refundRequest.CustomerReason
                        .Contains(search));
        }

        if ( request.Status.HasValue )
        {
            query =
                query.Where(refundRequest =>
                    refundRequest.Status ==
                    request.Status.Value);
        }

        if ( request.CustomerUserId.HasValue )
        {
            query =
                query.Where(refundRequest =>
                    refundRequest.Order.UserId ==
                    request.CustomerUserId.Value);
        }

        if ( request.OrderId.HasValue )
        {
            query =
                query.Where(refundRequest =>
                    refundRequest.OrderId ==
                    request.OrderId.Value);
        }

        if ( request.FromDateUtc.HasValue )
        {
            query =
                query.Where(refundRequest =>
                    refundRequest.RequestedAt >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            query =
                query.Where(refundRequest =>
                    refundRequest.RequestedAt <=
                    request.ToDateUtc.Value);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(refundRequest =>
                    refundRequest.RequestedAt)
                .ThenByDescending(refundRequest =>
                    refundRequest.Id)
                .Skip((int)skipCount)
                .Take(request.PageSize)
                .Select(refundRequest =>
                    new AdminRefundRequestListItemResponse
                    {
                        Id =
                            refundRequest.Id ,

                        OrderId =
                            refundRequest.OrderId ,

                        OrderNumber =
                            refundRequest.Order.OrderNumber ,

                        CustomerUserId =
                            refundRequest.Order.UserId ,

                        CustomerNameAr =
                            refundRequest.Order.CustomerNameAr ,

                        CustomerNameEn =
                            refundRequest.Order.CustomerNameEn ,

                        CustomerPhone =
                            refundRequest.Order.CustomerPhone ,

                        Status =
                            refundRequest.Status ,

                        CustomerReason =
                            refundRequest.CustomerReason ,

                        RefundAmount =
                            refundRequest.RefundAmount ,

                        ItemCount =
                            refundRequest.Items.Count(item =>
                                !item.IsDeleted) ,

                        TotalQuantity =
                            refundRequest.Items
                                .Where(item =>
                                    !item.IsDeleted)
                                .Select(item =>
                                    (int?)item.Quantity)
                                .Sum()
                            ?? 0 ,

                        RequestedAt =
                            refundRequest.RequestedAt ,

                        ReviewedAt =
                            refundRequest.ReviewedAt ,

                        ReviewedByEmployeeId =
                            refundRequest
                                .ReviewedByEmployeeId ,

                        CompletedAt =
                            refundRequest.CompletedAt ,

                        CompletedByEmployeeId =
                            refundRequest
                                .CompletedByEmployeeId
                    })
                .ToArrayAsync(
                    cancellationToken);

        var totalPages =
            totalCount == 0
                ? 0
                : (int)Math.Ceiling(
                    totalCount /
                    (double)request.PageSize);

        var response =
            new AdminRefundRequestListResponse
            {
                Items =
                    items ,

                PageNumber =
                    request.PageNumber ,

                PageSize =
                    request.PageSize ,

                TotalCount =
                    totalCount ,

                TotalPages =
                    totalPages
            };

        return RefundRequestResult<
            AdminRefundRequestListResponse>.Success(
                response);
    }

    public async Task<
        RefundRequestResult<AdminRefundRequestDetailsResponse>>
        GetAdminDetailsAsync(
            int refundRequestId ,
            CancellationToken cancellationToken = default )
    {
        if ( refundRequestId <= 0 )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The refund request identifier must be " +
                    "greater than zero.");
        }

        var refundRequest =
            await _dbContext.RefundRequests
                .AsNoTracking()
                .Include(candidate =>
                    candidate.Order)
                .Include(candidate =>
                    candidate.Items)
                .ThenInclude(item =>
                    item.OrderItem)
                .AsSplitQuery()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == refundRequestId &&
                        !candidate.IsDeleted &&
                        !candidate.Order.IsDeleted ,
                    cancellationToken);

        if ( refundRequest is null )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes
                        .RefundRequestNotFound ,
                    "The refund request was not found.");
        }

        return RefundRequestResult<
            AdminRefundRequestDetailsResponse>.Success(
                CreateAdminResponse(refundRequest));
    }

    private static string? ValidateAdminListRequest(
        GetAdminRefundRequestsRequest request )
    {
        if ( request.PageNumber <= 0 )
        {
            return
                "Page number must be greater than zero.";
        }

        if ( request.PageSize <= 0 ||
            request.PageSize > MaximumPageSize )
        {
            return
                $"Page size must be between 1 and " +
                $"{MaximumPageSize}.";
        }

        var search =
            request.Search?.Trim();

        if ( search?.Length > MaximumSearchLength )
        {
            return
                $"Search text cannot exceed " +
                $"{MaximumSearchLength} characters.";
        }

        if ( request.CustomerUserId.HasValue &&
            request.CustomerUserId.Value <= 0 )
        {
            return
                "The customer identifier must be greater " +
                "than zero.";
        }

        if ( request.OrderId.HasValue &&
            request.OrderId.Value <= 0 )
        {
            return
                "The order identifier must be greater " +
                "than zero.";
        }

        if ( request.Status.HasValue &&
            !Enum.IsDefined(request.Status.Value) )
        {
            return
                "The refund status is invalid.";
        }

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
            request.ToDateUtc.Value )
        {
            return
                "The start date cannot be later than " +
                "the end date.";
        }

        return null;
    }
}