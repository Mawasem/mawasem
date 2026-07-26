using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Contracts.Responses;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Reports;

public sealed partial class ReportService
{
    public async Task<
        ReportResult<SalesOverTimeResponse>>
        GetSalesOverTimeAsync(
            GetSalesOverTimeRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
                request.ToDateUtc.Value )
        {
            return ReportResult<
                SalesOverTimeResponse>.Failure(
                    ReportErrorCodes.InvalidDateRange ,
                    "The start date cannot be later than the end date.");
        }

        if ( !Enum.IsDefined(
                typeof(SalesReportGranularity) ,
                request.Granularity) )
        {
            return ReportResult<
                SalesOverTimeResponse>.Failure(
                    ReportErrorCodes.InvalidRequest ,
                    "The sales report granularity is invalid.");
        }

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

        var deliveredHistoryRows =
            await deliveredHistoryQuery
                .Select(history =>
                    new
                    {
                        history.OrderId ,

                        DeliveredAtUtc =
                            history.ChangedAtUtc ,

                        history.Order.TotalAmount
                    })
                .ToArrayAsync(
                    cancellationToken);

        // An order should normally have one Delivered transition.
        // Grouping prevents accidental duplicate history rows from
        // counting the same order more than once.
        var deliveredOrders =
            deliveredHistoryRows
                .GroupBy(row =>
                    row.OrderId)
                .Select(group =>
                    group
                        .OrderBy(row =>
                            row.DeliveredAtUtc)
                        .First())
                .ToArray();

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

        var completedRefunds =
            await completedRefundQuery
                .Select(refund =>
                    new
                    {
                        CompletedAtUtc =
                            refund.CompletedAt!.Value ,

                        refund.RefundAmount
                    })
                .ToArrayAsync(
                    cancellationToken);

        var deliveryGroups =
            deliveredOrders
                .GroupBy(delivery =>
                    GetSalesPeriodStartUtc(
                        delivery.DeliveredAtUtc ,
                        request.Granularity))
                .ToDictionary(
                    group =>
                        group.Key ,
                    group =>
                        new
                        {
                            DeliveredOrders =
                                group.Count() ,

                            GrossSales =
                                group.Sum(delivery =>
                                    delivery.TotalAmount)
                        });

        var refundGroups =
            completedRefunds
                .GroupBy(refund =>
                    GetSalesPeriodStartUtc(
                        refund.CompletedAtUtc ,
                        request.Granularity))
                .ToDictionary(
                    group =>
                        group.Key ,
                    group =>
                        group.Sum(refund =>
                            refund.RefundAmount));

        var periodStarts =
            deliveryGroups.Keys
                .Concat(
                    refundGroups.Keys)
                .Distinct()
                .OrderBy(periodStart =>
                    periodStart)
                .ToArray();

        var items =
            periodStarts
                .Select(periodStart =>
                {
                    var deliveredOrderCount =
                        0;

                    var grossSales =
                        0m;

                    if ( deliveryGroups.TryGetValue(
                            periodStart ,
                            out var deliveryGroup) )
                    {
                        deliveredOrderCount =
                            deliveryGroup.DeliveredOrders;

                        grossSales =
                            deliveryGroup.GrossSales;
                    }

                    var completedRefundAmount =
                        refundGroups.GetValueOrDefault(
                            periodStart);

                    return new SalesOverTimePointResponse
                    {
                        PeriodStartUtc =
                            periodStart ,

                        DeliveredOrders =
                            deliveredOrderCount ,

                        GrossSales =
                            grossSales ,

                        CompletedRefundAmount =
                            completedRefundAmount ,

                        NetRevenue =
                            grossSales -
                            completedRefundAmount
                    };
                })
                .ToArray();

        var totalGrossSales =
            items.Sum(item =>
                item.GrossSales);

        var totalCompletedRefundAmount =
            items.Sum(item =>
                item.CompletedRefundAmount);

        var response =
            new SalesOverTimeResponse
            {
                FromDateUtc =
                    request.FromDateUtc ,

                ToDateUtc =
                    request.ToDateUtc ,

                Granularity =
                    request.Granularity ,

                TotalDeliveredOrders =
                    items.Sum(item =>
                        item.DeliveredOrders) ,

                TotalGrossSales =
                    totalGrossSales ,

                TotalCompletedRefundAmount =
                    totalCompletedRefundAmount ,

                TotalNetRevenue =
                    totalGrossSales -
                    totalCompletedRefundAmount ,

                Items =
                    items
            };

        return ReportResult<
            SalesOverTimeResponse>.Success(
                response);
    }

    private static DateTime GetSalesPeriodStartUtc(
        DateTime timestampUtc ,
        SalesReportGranularity granularity )
    {
        return granularity switch
        {
            SalesReportGranularity.Day =>
                new DateTime(
                    timestampUtc.Year ,
                    timestampUtc.Month ,
                    timestampUtc.Day ,
                    0 ,
                    0 ,
                    0 ,
                    DateTimeKind.Utc) ,

            SalesReportGranularity.Month =>
                new DateTime(
                    timestampUtc.Year ,
                    timestampUtc.Month ,
                    1 ,
                    0 ,
                    0 ,
                    0 ,
                    DateTimeKind.Utc) ,

            _ =>
                throw new ArgumentOutOfRangeException(
                    nameof(granularity) ,
                    granularity ,
                    "Unsupported sales report granularity.")
        };
    }
}
