using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Domain.Common;
using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mawasem.Infrastructure.Refunds;

public sealed partial class RefundRequestService
{
    private async Task<RefundRequestDetailsResponse?>
        TryGetIdempotentReplayAsync(
            int orderId ,
            int customerUserId ,
            string idempotencyKey ,
            CancellationToken cancellationToken )
    {
        var existingRequest =
            await _dbContext.RefundRequests
                .AsNoTracking()
                .Include(refundRequest =>
                    refundRequest.Order)
                .Include(refundRequest =>
                    refundRequest.Items)
                .ThenInclude(refundRequestItem =>
                    refundRequestItem.OrderItem)
                .AsSplitQuery()
                .SingleOrDefaultAsync(
                    refundRequest =>
                        refundRequest.OrderId == orderId &&
                        refundRequest.Order.UserId ==
                            customerUserId &&
                        !refundRequest.Order.IsDeleted &&
                        !refundRequest.IsDeleted &&
                        refundRequest.IdempotencyKey ==
                            idempotencyKey ,
                    cancellationToken);

        return existingRequest is null
            ? null
            : CreateResponse(existingRequest);
    }

    private static RefundRequestDetailsResponse
        CreateResponse(
            RefundRequest refundRequest )
    {
        var items =
            refundRequest.Items
                .Where(item =>
                    !item.IsDeleted)
                .OrderBy(item =>
                    item.Id)
                .Select(
                    item =>
                    {
                        var orderItem =
                            item.OrderItem;

                        return new RefundRequestItemResponse
                        {
                            Id =
                                item.Id ,

                            OrderItemId =
                                item.OrderItemId ,

                            ProductId =
                                orderItem.ProductId ,

                            ProductVariantId =
                                orderItem.ProductVariantId ,

                            ProductNameAr =
                                orderItem.ProductNameAr ,

                            ProductNameEn =
                                orderItem.ProductNameEn ,

                            Sku =
                                orderItem.SKU ,

                            VariantSummaryAr =
                                orderItem.VariantSummaryAr ,

                            VariantSummaryEn =
                                orderItem.VariantSummaryEn ,

                            Quantity =
                                item.Quantity ,

                            ReturnedQuantity =
                                item.ReturnedQuantity ,

                            RestockQuantity =
                                item.RestockQuantity ,

                            Reason =
                                item.Reason ,

                            UnitRefundAmount =
                                item.UnitRefundAmount ,

                            TotalRefundAmount =
                                item.TotalRefundAmount
                        };
                    })
                .ToArray();

        return new RefundRequestDetailsResponse
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

            AdminNotes =
                refundRequest.AdminNotes ,

            RefundAmount =
                refundRequest.RefundAmount ,

            RequestedAt =
                refundRequest.RequestedAt ,

            ReviewedAt =
                refundRequest.ReviewedAt ,

            ReviewedByEmployeeId =
                refundRequest.ReviewedByEmployeeId ,

            CompletedAt =
                refundRequest.CompletedAt ,

            CompletedByEmployeeId =
                refundRequest.CompletedByEmployeeId ,

            StockRestoredAtUtc =
                refundRequest.StockRestoredAtUtc ,

            Items =
                items
        };
    }

    private static decimal CalculateUnitRefundAmount(
        OrderItem orderItem )
    {
        if ( orderItem.Quantity <= 0 )
        {
            throw new InvalidOperationException(
                "The source order item quantity is invalid.");
        }

        return decimal.Round(
            orderItem.TotalPrice / orderItem.Quantity ,
            2 ,
            MidpointRounding.AwayFromZero);
    }

    private static decimal CalculateTotalRefundAmount(
        decimal unitRefundAmount ,
        int quantity )
    {
        return decimal.Round(
            checked(unitRefundAmount * quantity) ,
            2 ,
            MidpointRounding.AwayFromZero);
    }

    private static string? NormalizeRequiredText(
        string? value )
    {
        if ( string.IsNullOrWhiteSpace(value) )
        {
            return null;
        }

        return value.Trim();
    }

    private static string? NormalizeOptionalText(
        string? value )
    {
        if ( string.IsNullOrWhiteSpace(value) )
        {
            return null;
        }

        return value.Trim();
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