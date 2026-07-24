using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mawasem.Infrastructure.Orders;

public sealed partial class OrderWorkflowService
{
    public async Task<
        OrderWorkflowResult<OrderWorkflowResponse>>
        ConfirmAsync(
            int orderId ,
            int dashboardUserId ,
            CancellationToken cancellationToken = default )
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable ,
                cancellationToken);

        try
        {
            var order = await _dbContext.Orders
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

            if ( previousStatus ==
                OrderStatus.Confirmed )
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

            if ( previousStatus !=
                OrderStatus.Pending )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return OrderWorkflowResult<
                    OrderWorkflowResponse>.Failure(
                        OrderWorkflowErrorCodes
                            .InvalidStatusTransition ,

                        $"An order with status " +
                        $"'{previousStatus}' cannot be confirmed.");
            }

            var now =
                _timeProvider.GetUtcNow();

            var actor =
                GetActor(dashboardUserId);

            order.OrderStatus =
                OrderStatus.Confirmed;

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
                    "The order changed while it was being confirmed.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return OrderWorkflowResult<
                OrderWorkflowResponse>.Failure(
                    OrderWorkflowErrorCodes.OperationFailed ,
                    "The order could not be confirmed.");
        }
    }
}