using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Application.Features.Refunds.Models;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Refunds;

public sealed partial class RefundRequestService
{
    private const int MaximumPageSize = 100;

    private const int MaximumSearchLength = 100;

    public async Task<
        RefundRequestResult<CustomerRefundRequestListResponse>>
        GetCustomerListAsync(
            int customerUserId ,
            GetCustomerRefundRequestsRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( customerUserId <= 0 )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( request.PageNumber <= 0 )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "Page number must be greater than zero.");
        }

        if ( request.PageSize <= 0 ||
            request.PageSize > MaximumPageSize )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    $"Page size must be between 1 and " +
                    $"{MaximumPageSize}.");
        }

        var skipCount =
            (long)( request.PageNumber - 1 ) *
            request.PageSize;

        if ( skipCount > int.MaxValue )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The requested page is outside the " +
                    "supported range.");
        }

        if ( request.Status.HasValue &&
            !Enum.IsDefined(request.Status.Value) )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The refund status is invalid.");
        }

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
            request.ToDateUtc.Value )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The start date cannot be later than " +
                    "the end date.");
        }

        var search =
            request.Search?.Trim();

        if ( search?.Length > MaximumSearchLength )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    $"Search text cannot exceed " +
                    $"{MaximumSearchLength} characters.");
        }

        var customer =
            await _dbContext.Users
                .AsNoTracking()
                .Where(user =>
                    user.Id == customerUserId)
                .Select(user =>
                    new
                    {
                        user.IsBlocked
                    })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( customer is null )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( customer.IsBlocked )
        {
            return RefundRequestResult<
                CustomerRefundRequestListResponse>.Failure(
                    RefundRequestErrorCodes.CustomerBlocked ,
                    "The customer account is blocked.");
        }

        var query =
            _dbContext.RefundRequests
                .AsNoTracking()
                .Where(refundRequest =>
                    !refundRequest.IsDeleted &&
                    !refundRequest.Order.IsDeleted &&
                    refundRequest.Order.UserId ==
                        customerUserId);

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            query =
                query.Where(refundRequest =>
                    refundRequest.Order.OrderNumber
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
                    new CustomerRefundRequestListItemResponse
                    {
                        Id =
                            refundRequest.Id ,

                        OrderId =
                            refundRequest.OrderId ,

                        OrderNumber =
                            refundRequest.Order.OrderNumber ,

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
                                .Sum(item =>
                                    (int?)item.Quantity)
                            ?? 0 ,

                        RequestedAt =
                            refundRequest.RequestedAt ,

                        ReviewedAt =
                            refundRequest.ReviewedAt ,

                        CompletedAt =
                            refundRequest.CompletedAt
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
            new CustomerRefundRequestListResponse
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
            CustomerRefundRequestListResponse>.Success(
                response);
    }

    public async Task<
        RefundRequestResult<RefundRequestDetailsResponse>>
        GetCustomerDetailsAsync(
            int customerUserId ,
            int refundRequestId ,
            CancellationToken cancellationToken = default )
    {
        if ( customerUserId <= 0 )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( refundRequestId <= 0 )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The refund request identifier is invalid.");
        }

        var customer =
            await _dbContext.Users
                .AsNoTracking()
                .Where(user =>
                    user.Id == customerUserId)
                .Select(user =>
                    new
                    {
                        user.IsBlocked
                    })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( customer is null )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( customer.IsBlocked )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.CustomerBlocked ,
                    "The customer account is blocked.");
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
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes
                        .RefundRequestNotFound ,
                    "The refund request was not found.");
        }

        if ( refundRequest.Order.UserId !=
            customerUserId )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes
                        .RefundRequestAccessDenied ,
                    "The customer is not authorized to " +
                    "access this refund request.");
        }

        return RefundRequestResult<
            RefundRequestDetailsResponse>.Success(
                CreateResponse(refundRequest));
    }
}