using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mawasem.Infrastructure.Orders;

public sealed partial class OrderWorkflowService
{
    public Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        RejectAsync(
            int orderId ,
            int dashboardUserId ,
            string reason ,
            CancellationToken cancellationToken = default )
    {
        return RestoreAndTransitionAsync(
            orderId ,
            dashboardUserId ,
            customerUserId: null ,
            OrderStatus.Rejected ,
            reason ,
            allowConfirmedCancellation: false ,
            cancellationToken);
    }

    public Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        CancelByDashboardAsync(
            int orderId ,
            int dashboardUserId ,
            string reason ,
            CancellationToken cancellationToken = default )
    {
        return RestoreAndTransitionAsync(
            orderId ,
            dashboardUserId ,
            customerUserId: null ,
            OrderStatus.Cancelled ,
            reason ,
            allowConfirmedCancellation: true ,
            cancellationToken);
    }

    public Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        CancelByCustomerAsync(
            int orderId ,
            int customerUserId ,
            string reason ,
            CancellationToken cancellationToken = default )
    {
        return RestoreAndTransitionAsync(
            orderId ,
            customerUserId ,
            customerUserId ,
            OrderStatus.Cancelled ,
            reason ,
            allowConfirmedCancellation: false ,
            cancellationToken);
    }

    private async Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        RestoreAndTransitionAsync(
            int orderId ,
            int actorUserId ,
            int? customerUserId ,
            OrderStatus targetStatus ,
            string reason ,
            bool allowConfirmedCancellation ,
            CancellationToken cancellationToken )
    {
        var normalizedReason =
            NormalizeReason(reason);

        if ( normalizedReason is null ||
            normalizedReason.Length > MaxReasonLength )
        {
            return OrderWorkflowResult<
                OrderWorkflowResponse>.Failure(
                    OrderWorkflowErrorCodes.InvalidReason ,
                    $"A reason is required and cannot exceed " +
                    $"{MaxReasonLength} characters.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable ,
                cancellationToken);

        try
        {
            var order = await _dbContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                    .ThenInclude(item =>
                        item.ProductVariant)
                .AsSplitQuery()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == orderId &&
                        !candidate.IsDeleted ,
                    cancellationToken);

            if ( order is null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return OrderWorkflowResult<
                    OrderWorkflowResponse>.Failure(
                        OrderWorkflowErrorCodes.OrderNotFound ,
                        "The order was not found.");
            }

            if ( customerUserId.HasValue &&
                order.UserId != customerUserId.Value )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return OrderWorkflowResult<
                    OrderWorkflowResponse>.Failure(
                        OrderWorkflowErrorCodes
                            .OrderAccessDenied ,

                        "The order does not belong to the customer.");
            }

            var previousStatus =
                order.OrderStatus;

            var statusAlreadyApplied =
                previousStatus == targetStatus;

            var pendingTransition =
                previousStatus ==
                OrderStatus.Pending;

            var confirmedCancellation =
                targetStatus ==
                    OrderStatus.Cancelled &&
                allowConfirmedCancellation &&
                previousStatus ==
                    OrderStatus.Confirmed;

            if ( !statusAlreadyApplied &&
                !pendingTransition &&
                !confirmedCancellation )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return OrderWorkflowResult<
                    OrderWorkflowResponse>.Failure(
                        OrderWorkflowErrorCodes
                            .InvalidStatusTransition ,

                        $"An order with status " +
                        $"'{previousStatus}' cannot transition " +
                        $"to '{targetStatus}'.");
            }

            var now =
                _timeProvider.GetUtcNow();

            var actor =
                GetActor(actorUserId);

            var stockRestored =
                false;

            if ( !order.StockRestoredAtUtc.HasValue )
            {
                RestoreStock(
                    order ,
                    now ,
                    actor);

                order.StockRestoredAtUtc =
                    now.UtcDateTime;

                stockRestored =
                    true;
            }

            var statusChanged =
                previousStatus != targetStatus;

            if ( statusChanged )
            {
                order.OrderStatus =
                    targetStatus;

                if ( targetStatus ==
                    OrderStatus.Rejected )
                {
                    order.RejectionReason =
                        normalizedReason;

                    order.RejectedAtUtc =
                        now.UtcDateTime;
                }
                else
                {
                    order.CancellationReason =
                        normalizedReason;

                    order.CancelledAtUtc =
                        now.UtcDateTime;
                }
            }

            if ( statusChanged ||
                stockRestored )
            {
                MarkModified(
                    order ,
                    now ,
                    actor);

                await _dbContext.SaveChangesAsync(
                    cancellationToken);
            }

            await transaction.CommitAsync(
                cancellationToken);

            return OrderWorkflowResult<
                OrderWorkflowResponse>.Success(
                    CreateResponse(
                        order ,
                        previousStatus ,
                        statusChanged ,
                        stockRestored));
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return OrderWorkflowResult<
                OrderWorkflowResponse>.Failure(
                    OrderWorkflowErrorCodes.ConcurrencyConflict ,
                    "Stock or order data changed while the " +
                    "operation was being completed.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return OrderWorkflowResult<
                OrderWorkflowResponse>.Failure(
                    OrderWorkflowErrorCodes.OperationFailed ,
                    "The order workflow operation could not be completed.");
        }
    }
}