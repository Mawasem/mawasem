using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Contracts.Responses;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Reports;

public sealed partial class ReportService
{
    public async Task<
        ReportResult<BusinessDashboardResponse>>
        GetBusinessDashboardAsync(
            GetBusinessDashboardRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
                request.ToDateUtc.Value )
        {
            return ReportResult<
                BusinessDashboardResponse>.Failure(
                    ReportErrorCodes.InvalidDateRange ,
                    "The start date cannot be later than the end date.");
        }

        var orderQuery =
            _dbContext.Orders
                .AsNoTracking()
                .Where(order =>
                    !order.IsDeleted);

        if ( request.FromDateUtc.HasValue )
        {
            orderQuery =
                orderQuery.Where(order =>
                    order.OrderDate >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            orderQuery =
                orderQuery.Where(order =>
                    order.OrderDate <=
                    request.ToDateUtc.Value);
        }

        var totalOrders =
            await orderQuery.CountAsync(
                cancellationToken);

        var statusRows =
            await orderQuery
                .GroupBy(order =>
                    order.OrderStatus)
                .Select(group =>
                    new
                    {
                        Status =
                            group.Key ,

                        Count =
                            group.Count()
                    })
                .ToArrayAsync(
                    cancellationToken);

        var statusCounts =
            statusRows.ToDictionary(
                row =>
                    row.Status ,
                row =>
                    row.Count);

        var orderStatusCounts =
            Enum.GetValues<OrderStatus>()
                .Select(status =>
                    new OrderStatusCountResponse
                    {
                        Status =
                            status ,

                        Count =
                            statusCounts.GetValueOrDefault(
                                status)
                    })
                .ToArray();

        var pendingFulfillmentOrders =
            statusCounts.GetValueOrDefault(
                OrderStatus.Pending) +
            statusCounts.GetValueOrDefault(
                OrderStatus.Confirmed) +
            statusCounts.GetValueOrDefault(
                OrderStatus.Preparing) +
            statusCounts.GetValueOrDefault(
                OrderStatus.Shipped);

        var deliveredHistoryQuery =
            _dbContext.OrderStatusHistories
                .AsNoTracking()
                .Where(history =>
                    history.NewStatus ==
                        OrderStatus.Delivered &&
                    !history.Order.IsDeleted);

        if ( request.FromDateUtc.HasValue )
        {
            deliveredHistoryQuery =
                deliveredHistoryQuery.Where(history =>
                    history.ChangedAtUtc >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            deliveredHistoryQuery =
                deliveredHistoryQuery.Where(history =>
                    history.ChangedAtUtc <=
                    request.ToDateUtc.Value);
        }

        var deliveredSalesQuery =
            deliveredHistoryQuery
                .Select(history =>
                    new
                    {
                        history.OrderId ,

                        history.Order.TotalAmount
                    })
                .Distinct();

        var deliveredSales =
            await deliveredSalesQuery
                .ToArrayAsync(
                    cancellationToken);

        var deliveredOrders =
            deliveredSales.Length;

        var grossSales =
            deliveredSales.Sum(delivery =>
                delivery.TotalAmount);

        var completedRefundQuery =
            _dbContext.RefundRequests
                .AsNoTracking()
                .Where(refund =>
                    !refund.IsDeleted &&
                    !refund.Order.IsDeleted &&
                    refund.Status ==
                        RefundStatus.Completed &&
                    refund.CompletedAt.HasValue);

        if ( request.FromDateUtc.HasValue )
        {
            completedRefundQuery =
                completedRefundQuery.Where(refund =>
                    refund.CompletedAt!.Value >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            completedRefundQuery =
                completedRefundQuery.Where(refund =>
                    refund.CompletedAt!.Value <=
                    request.ToDateUtc.Value);
        }

        var completedRefundAmounts =
            await completedRefundQuery
                .Select(refund =>
                    refund.RefundAmount)
                .ToArrayAsync(
                    cancellationToken);

        var completedRefundAmount =
            completedRefundAmounts.Sum();

        var netRevenue =
            grossSales -
            completedRefundAmount;

        var averageOrderValue =
            deliveredOrders == 0
                ? 0m
                : decimal.Round(
                    grossSales /
                    deliveredOrders ,
                    2 ,
                    MidpointRounding.AwayFromZero);

        var response =
            new BusinessDashboardResponse
            {
                FromDateUtc =
                    request.FromDateUtc ,

                ToDateUtc =
                    request.ToDateUtc ,

                TotalOrders =
                    totalOrders ,

                DeliveredOrders =
                    deliveredOrders ,

                PendingFulfillmentOrders =
                    pendingFulfillmentOrders ,

                CancelledOrders =
                    statusCounts.GetValueOrDefault(
                        OrderStatus.Cancelled) ,

                RejectedOrders =
                    statusCounts.GetValueOrDefault(
                        OrderStatus.Rejected) ,

                GrossSales =
                    grossSales ,

                CompletedRefundAmount =
                    completedRefundAmount ,

                NetRevenue =
                    netRevenue ,

                AverageOrderValue =
                    averageOrderValue ,

                OrderStatusCounts =
                    orderStatusCounts
            };

        return ReportResult<
            BusinessDashboardResponse>.Success(
                response);
    }
}
