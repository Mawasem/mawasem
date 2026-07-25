using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Application.Features.Refunds.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mawasem.Infrastructure.Refunds;

public sealed partial class RefundRequestService
{
    private const int MaximumAdminNotesLength = 2000;

    public Task<
        RefundRequestResult<AdminRefundRequestDetailsResponse>>
        ApproveAsync(
            int refundRequestId ,
            int dashboardUserId ,
            ApproveRefundRequestRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var adminNotes =
            NormalizeOptionalText(request.AdminNotes);

        if ( adminNotes?.Length >
            MaximumAdminNotesLength )
        {
            return Task.FromResult(
                RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidAdminNotes ,
                        $"Admin notes cannot exceed " +
                        $"{MaximumAdminNotesLength} characters."));
        }

        return ReviewAsync(
            refundRequestId ,
            dashboardUserId ,
            RefundStatus.Approved ,
            adminNotes ,
            cancellationToken);
    }

    public Task<
        RefundRequestResult<AdminRefundRequestDetailsResponse>>
        RejectAsync(
            int refundRequestId ,
            int dashboardUserId ,
            RejectRefundRequestRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var adminNotes =
            NormalizeRequiredText(request.AdminNotes);

        if ( adminNotes is null ||
            adminNotes.Length >
            MaximumAdminNotesLength )
        {
            return Task.FromResult(
                RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidAdminNotes ,
                        $"Admin notes are required and cannot exceed " +
                        $"{MaximumAdminNotesLength} characters."));
        }

        return ReviewAsync(
            refundRequestId ,
            dashboardUserId ,
            RefundStatus.Rejected ,
            adminNotes ,
            cancellationToken);
    }

    private async Task<
        RefundRequestResult<AdminRefundRequestDetailsResponse>>
        ReviewAsync(
            int refundRequestId ,
            int dashboardUserId ,
            RefundStatus targetStatus ,
            string? adminNotes ,
            CancellationToken cancellationToken )
    {
        if ( refundRequestId <= 0 )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The refund request identifier must be " +
                    "greater than zero.");
        }

        if ( dashboardUserId <= 0 )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The dashboard user identifier must be " +
                    "greater than zero.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable ,
                cancellationToken);

        try
        {
            var refundRequest =
                await _dbContext.RefundRequests
                    .Include(candidate =>
                        candidate.Order)
                    .ThenInclude(order =>
                        order.OrderItems)
                    .Include(candidate =>
                        candidate.Order)
                    .ThenInclude(order =>
                        order.RefundRequests)
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
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes
                            .RefundRequestNotFound ,
                        "The refund request was not found.");
            }

            if ( refundRequest.Status == targetStatus )
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Success(
                        CreateAdminResponse(refundRequest));
            }

            if ( refundRequest.Status !=
                RefundStatus.Pending )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes
                            .InvalidStatusTransition ,
                        $"A refund request with status " +
                        $"'{refundRequest.Status}' cannot transition " +
                        $"to '{targetStatus}'.");
            }

            var now =
                _timeProvider.GetUtcNow();

            var actor =
                GetActor(dashboardUserId);

            refundRequest.Status =
                targetStatus;

            refundRequest.AdminNotes =
                adminNotes;

            refundRequest.ReviewedAt =
                now.UtcDateTime;

            refundRequest.ReviewedByEmployeeId =
                dashboardUserId;

            MarkModified(
                refundRequest ,
                now ,
                actor);

            if ( targetStatus ==
                RefundStatus.Rejected )
            {
                UpdateOrderStatusAfterRejection(
                    refundRequest ,
                    now ,
                    actor);
            }

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Success(
                    CreateAdminResponse(refundRequest));
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.ConcurrencyConflict ,
                    "The refund request changed while it was " +
                    "being reviewed.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.OperationFailed ,
                    "The refund request review could not be saved.");
        }
    }

    private static void UpdateOrderStatusAfterRejection(
        RefundRequest rejectedRequest ,
        DateTimeOffset now ,
        string actor )
    {
        var order =
            rejectedRequest.Order;

        var hasAnotherActiveRefundRequest =
            order.RefundRequests.Any(
                candidate =>
                    candidate.Id != rejectedRequest.Id &&
                    !candidate.IsDeleted &&
                    candidate.Status is
                        RefundStatus.Pending or
                        RefundStatus.Approved);

        var activeOrderItems =
            order.OrderItems
                .Where(item =>
                    !item.IsDeleted)
                .ToArray();

        var isFullyRefunded =
            activeOrderItems.Length > 0 &&
            activeOrderItems.All(item =>
                item.RefundedQuantity >=
                item.Quantity);

        var isPartiallyRefunded =
            activeOrderItems.Any(item =>
                item.RefundedQuantity > 0);

        var targetOrderStatus =
            isFullyRefunded
                ? OrderStatus.Refunded
                : hasAnotherActiveRefundRequest
                    ? OrderStatus.RefundRequested
                    : isPartiallyRefunded
                        ? OrderStatus.PartiallyRefunded
                        : OrderStatus.Delivered;

        if ( order.OrderStatus ==
            targetOrderStatus )
        {
            return;
        }

        order.OrderStatus =
            targetOrderStatus;

        MarkModified(
            order ,
            now ,
            actor);
    }
}