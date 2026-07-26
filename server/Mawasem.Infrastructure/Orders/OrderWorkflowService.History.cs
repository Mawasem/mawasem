using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;

namespace Mawasem.Infrastructure.Orders;

public sealed partial class OrderWorkflowService
{
    private void RecordStatusChange(
        Order order ,
        OrderStatus previousStatus ,
        OrderStatus newStatus ,
        int? changedByUserId ,
        OrderStatusChangeActorType actorType ,
        DateTimeOffset changedAt ,
        string? reason = null )
    {
        ArgumentNullException.ThrowIfNull(order);

        if ( previousStatus == newStatus )
        {
            throw new InvalidOperationException(
                "An order status-history record requires a status change.");
        }

        var requiresUser =
            actorType is
                OrderStatusChangeActorType.Customer or
                OrderStatusChangeActorType.DashboardUser;

        if ( requiresUser &&
            !changedByUserId.HasValue )
        {
            throw new InvalidOperationException(
                "Customer and dashboard status changes require a user.");
        }

        if ( actorType ==
                OrderStatusChangeActorType.System &&
            changedByUserId.HasValue )
        {
            throw new InvalidOperationException(
                "A system status change cannot contain a user.");
        }

        var normalizedReason =
            string.IsNullOrWhiteSpace(reason)
                ? null
                : reason.Trim();

        _dbContext.OrderStatusHistories.Add(
            new OrderStatusHistory
            {
                OrderId =
                    order.Id ,

                PreviousStatus =
                    previousStatus ,

                NewStatus =
                    newStatus ,

                ChangedByUserId =
                    changedByUserId ,

                ActorType =
                    actorType ,

                ChangedAtUtc =
                    changedAt.UtcDateTime ,

                Reason =
                    normalizedReason
            });
    }
}