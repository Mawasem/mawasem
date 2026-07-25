using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Domain.Orders;

namespace Mawasem.Infrastructure.Refunds;

public sealed partial class RefundRequestService
{
    private static AdminRefundRequestDetailsResponse
        CreateAdminResponse(
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

        return new AdminRefundRequestDetailsResponse
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
}