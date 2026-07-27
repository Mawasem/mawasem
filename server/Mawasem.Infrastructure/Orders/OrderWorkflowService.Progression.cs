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
        PrepareAsync(
            int orderId ,
            int dashboardUserId ,
            CancellationToken cancellationToken = default )
    {
        return TransitionForwardAsync(
            orderId ,
            dashboardUserId ,
            expectedStatus:
                OrderStatus.Confirmed ,
            targetStatus:
                OrderStatus.Preparing ,
            cancellationToken);
    }

    public Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        ShipAsync(
            int orderId ,
            int dashboardUserId ,
            CancellationToken cancellationToken = default )
    {
        return TransitionForwardAsync(
            orderId ,
            dashboardUserId ,
            expectedStatus:
                OrderStatus.Preparing ,
            targetStatus:
                OrderStatus.Shipped ,
            cancellationToken);
    }

    public Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        DeliverAsync(
            int orderId ,
            int dashboardUserId ,
            CancellationToken cancellationToken = default )
    {
        return TransitionForwardAsync(
            orderId ,
            dashboardUserId ,
            expectedStatus:
                OrderStatus.Shipped ,
            targetStatus:
                OrderStatus.Delivered ,
            cancellationToken);
    }

    private async Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        TransitionForwardAsync(
            int orderId ,
            int dashboardUserId ,
            OrderStatus expectedStatus ,
            OrderStatus targetStatus ,
            CancellationToken cancellationToken )
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable ,
                cancellationToken);

        try
        {
            var order =
                await _dbContext.Orders
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

            var previousStatus =
                order.OrderStatus;

            if ( previousStatus == targetStatus )
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return OrderWorkflowResult<
                    OrderWorkflowResponse>.Success(
                        CreateResponse(
                            order ,
                            previousStatus ,
                            statusChanged: false ,
                            stockRestored: false));
            }

            if ( previousStatus != expectedStatus )
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
                GetActor(dashboardUserId);

            order.OrderStatus =
                targetStatus;

            if ( targetStatus ==
                    OrderStatus.Delivered &&
                 order.PaymentMethod ==
                    PaymentMethod.CashOnDelivery )
            {
                order.PaymentStatus =
                    PaymentStatus.Paid;
            }

            RecordStatusChange(
                order ,
                previousStatus ,
                targetStatus ,
                dashboardUserId ,
                OrderStatusChangeActorType.DashboardUser ,
                now);

            MarkModified(
                order ,
                now ,
                actor);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return OrderWorkflowResult<
                OrderWorkflowResponse>.Success(
                    CreateResponse(
                        order ,
                        previousStatus ,
                        statusChanged: true ,
                        stockRestored: false));
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return OrderWorkflowResult<
                OrderWorkflowResponse>.Failure(
                    OrderWorkflowErrorCodes.ConcurrencyConflict ,
                    "The order changed while its status was being updated.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return OrderWorkflowResult<
                OrderWorkflowResponse>.Failure(
                    OrderWorkflowErrorCodes.OperationFailed ,
                    "The order status could not be updated.");
        }
    }
}
