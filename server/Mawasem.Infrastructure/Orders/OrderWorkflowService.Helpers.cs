using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Domain.Common;
using Mawasem.Domain.Orders;
using System.Globalization;

namespace Mawasem.Infrastructure.Orders;

public sealed partial class OrderWorkflowService
{
    private static void RestoreStock(
        Order order ,
        DateTimeOffset now ,
        string actor )
    {
        var itemGroups = order.OrderItems
            .Where(item => !item.IsDeleted)
            .GroupBy(item =>
                item.ProductVariantId);

        foreach ( var itemGroup in itemGroups )
        {
            var variant =
                itemGroup.First().ProductVariant;

            var quantityToRestore =
                itemGroup.Sum(item =>
                    item.Quantity);

            variant.StockQuantity =
                checked(
                    variant.StockQuantity +
                    quantityToRestore);

            MarkModified(
                variant ,
                now ,
                actor);
        }
    }

    private static OrderWorkflowResponse
        CreateResponse(
            Order order ,
            Domain.Enums.OrderStatus previousStatus ,
            bool statusChanged ,
            bool stockRestored )
    {
        return new OrderWorkflowResponse
        {
            OrderId =
                order.Id ,

            OrderNumber =
                order.OrderNumber ,

            PreviousStatus =
                previousStatus ,

            CurrentStatus =
                order.OrderStatus ,

            StatusChanged =
                statusChanged ,

            StockRestored =
                stockRestored ,

            StockRestoredAtUtc =
                order.StockRestoredAtUtc
        };
    }

    private static string? NormalizeReason(
        string? reason )
    {
        if ( string.IsNullOrWhiteSpace(reason) )
        {
            return null;
        }

        return reason.Trim();
    }

    private static string GetActor(
        int userId )
    {
        return userId.ToString(
            CultureInfo.InvariantCulture);
    }

    private static void MarkModified(
        BaseAuditableEntity entity ,
        DateTimeOffset now ,
        string actor )
    {
        entity.LastModifiedOn =
            now;

        entity.LastModifiedBy =
            actor;
    }
}