using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Reports;
using Mawasem.Tests.Checkout;

namespace Mawasem.Tests.Reports;

public sealed class SalesOverTimeReportTests
{
    private static readonly DateTime BaseTimeUtc =
        new(
            2030 ,
            1 ,
            30 ,
            0 ,
            0 ,
            0 ,
            DateTimeKind.Utc);

    [Fact]
    public async Task
        GetSalesOverTimeAsync_DayGranularity_ReturnsExpectedPeriods()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedSalesScenarioAsync(
            database);

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetSalesOverTimeAsync(
                    new GetSalesOverTimeRequest
                    {
                        FromDateUtc =
                            BaseTimeUtc ,

                        ToDateUtc =
                            BaseTimeUtc.AddDays(3) ,

                        Granularity =
                            SalesReportGranularity.Day
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        var response =
            result.Response!;

        Assert.Equal(
            SalesReportGranularity.Day ,
            response.Granularity);

        Assert.Equal(
            2 ,
            response.TotalDeliveredOrders);

        Assert.Equal(
            300m ,
            response.TotalGrossSales);

        Assert.Equal(
            65m ,
            response.TotalCompletedRefundAmount);

        Assert.Equal(
            235m ,
            response.TotalNetRevenue);

        Assert.Collection(
            response.Items ,
            item =>
            {
                Assert.Equal(
                    BaseTimeUtc ,
                    item.PeriodStartUtc);

                Assert.Equal(
                    1 ,
                    item.DeliveredOrders);

                Assert.Equal(
                    100m ,
                    item.GrossSales);

                Assert.Equal(
                    25m ,
                    item.CompletedRefundAmount);

                Assert.Equal(
                    75m ,
                    item.NetRevenue);
            } ,
            item =>
            {
                Assert.Equal(
                    BaseTimeUtc.AddDays(1) ,
                    item.PeriodStartUtc);

                Assert.Equal(
                    1 ,
                    item.DeliveredOrders);

                Assert.Equal(
                    200m ,
                    item.GrossSales);

                Assert.Equal(
                    0m ,
                    item.CompletedRefundAmount);

                Assert.Equal(
                    200m ,
                    item.NetRevenue);
            } ,
            item =>
            {
                Assert.Equal(
                    BaseTimeUtc.AddDays(2) ,
                    item.PeriodStartUtc);

                Assert.Equal(
                    0 ,
                    item.DeliveredOrders);

                Assert.Equal(
                    0m ,
                    item.GrossSales);

                Assert.Equal(
                    40m ,
                    item.CompletedRefundAmount);

                Assert.Equal(
                    -40m ,
                    item.NetRevenue);
            });
    }

    [Fact]
    public async Task
        GetSalesOverTimeAsync_MonthGranularity_ReturnsExpectedPeriods()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedSalesScenarioAsync(
            database);

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetSalesOverTimeAsync(
                    new GetSalesOverTimeRequest
                    {
                        FromDateUtc =
                            BaseTimeUtc ,

                        ToDateUtc =
                            BaseTimeUtc.AddDays(3) ,

                        Granularity =
                            SalesReportGranularity.Month
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        var response =
            result.Response!;

        Assert.Equal(
            SalesReportGranularity.Month ,
            response.Granularity);

        Assert.Equal(
            2 ,
            response.TotalDeliveredOrders);

        Assert.Equal(
            300m ,
            response.TotalGrossSales);

        Assert.Equal(
            65m ,
            response.TotalCompletedRefundAmount);

        Assert.Equal(
            235m ,
            response.TotalNetRevenue);

        Assert.Collection(
            response.Items ,
            item =>
            {
                Assert.Equal(
                    new DateTime(
                        2030 ,
                        1 ,
                        1 ,
                        0 ,
                        0 ,
                        0 ,
                        DateTimeKind.Utc) ,
                    item.PeriodStartUtc);

                Assert.Equal(
                    2 ,
                    item.DeliveredOrders);

                Assert.Equal(
                    300m ,
                    item.GrossSales);

                Assert.Equal(
                    25m ,
                    item.CompletedRefundAmount);

                Assert.Equal(
                    275m ,
                    item.NetRevenue);
            } ,
            item =>
            {
                Assert.Equal(
                    new DateTime(
                        2030 ,
                        2 ,
                        1 ,
                        0 ,
                        0 ,
                        0 ,
                        DateTimeKind.Utc) ,
                    item.PeriodStartUtc);

                Assert.Equal(
                    0 ,
                    item.DeliveredOrders);

                Assert.Equal(
                    0m ,
                    item.GrossSales);

                Assert.Equal(
                    40m ,
                    item.CompletedRefundAmount);

                Assert.Equal(
                    -40m ,
                    item.NetRevenue);
            });
    }

    [Fact]
    public async Task
        GetSalesOverTimeAsync_InvalidDateRange_ReturnsFailure()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetSalesOverTimeAsync(
                    new GetSalesOverTimeRequest
                    {
                        FromDateUtc =
                            BaseTimeUtc.AddDays(1) ,

                        ToDateUtc =
                            BaseTimeUtc ,

                        Granularity =
                            SalesReportGranularity.Day
                    });

        Assert.False(
            result.Succeeded);

        Assert.Equal(
            ReportErrorCodes.InvalidDateRange ,
            result.ErrorCode);

        Assert.Null(
            result.Response);
    }

    [Fact]
    public async Task
        GetSalesOverTimeAsync_InvalidGranularity_ReturnsFailure()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetSalesOverTimeAsync(
                    new GetSalesOverTimeRequest
                    {
                        Granularity =
                            (SalesReportGranularity)999
                    });

        Assert.False(
            result.Succeeded);

        Assert.Equal(
            ReportErrorCodes.InvalidRequest ,
            result.ErrorCode);

        Assert.Null(
            result.Response);
    }

    private static async Task SeedSalesScenarioAsync(
        CheckoutTestDatabase database )
    {
        await using var dbContext =
            database.CreateContext();

        var firstDeliveredOrder =
            CreateOrder(
                "SALES-001" ,
                100m ,
                BaseTimeUtc);

        var secondDeliveredOrder =
            CreateOrder(
                "SALES-002" ,
                200m ,
                BaseTimeUtc.AddDays(1));

        var deletedDeliveredOrder =
            CreateOrder(
                "SALES-003" ,
                300m ,
                BaseTimeUtc.AddDays(1) ,
                isDeleted: true);

        dbContext.Orders.AddRange(
            firstDeliveredOrder ,
            secondDeliveredOrder ,
            deletedDeliveredOrder);

        await dbContext.SaveChangesAsync();

        dbContext.OrderStatusHistories.AddRange(
            CreateDeliveredHistory(
                firstDeliveredOrder ,
                BaseTimeUtc.AddHours(10)) ,

            CreateDeliveredHistory(
                firstDeliveredOrder ,
                BaseTimeUtc.AddHours(11)) ,

            CreateDeliveredHistory(
                secondDeliveredOrder ,
                BaseTimeUtc.AddDays(1).AddHours(12)) ,

            CreateDeliveredHistory(
                deletedDeliveredOrder ,
                BaseTimeUtc.AddDays(1).AddHours(13)));

        dbContext.RefundRequests.AddRange(
            CreateCompletedRefund(
                firstDeliveredOrder ,
                "sales-refund-1" ,
                25m ,
                BaseTimeUtc.AddHours(15)) ,

            CreateCompletedRefund(
                secondDeliveredOrder ,
                "sales-refund-2" ,
                40m ,
                BaseTimeUtc.AddDays(2).AddHours(9)) ,

            CreateCompletedRefund(
                secondDeliveredOrder ,
                "sales-refund-3" ,
                10m ,
                BaseTimeUtc.AddDays(1).AddHours(9) ,
                isDeleted: true) ,

            CreateCompletedRefund(
                deletedDeliveredOrder ,
                "sales-refund-4" ,
                20m ,
                BaseTimeUtc.AddDays(1).AddHours(10)));

        await dbContext.SaveChangesAsync();
    }

    private static Order CreateOrder(
        string orderNumber ,
        decimal totalAmount ,
        DateTime orderDateUtc ,
        bool isDeleted = false )
    {
        return new Order
        {
            UserId =
                CheckoutTestDatabase.CustomerId ,

            CustomerNameAr =
                "عميل تقرير المبيعات" ,

            CustomerNameEn =
                "Sales Report Customer" ,

            CustomerPhone =
                "01000000002" ,

            OrderNumber =
                orderNumber ,

            OrderDate =
                orderDateUtc ,

            SubTotal =
                totalAmount ,

            Discount =
                0m ,

            DeliveryFee =
                0m ,

            TotalAmount =
                totalAmount ,

            OrderStatus =
                OrderStatus.Delivered ,

            PaymentMethod =
                PaymentMethod.CashOnDelivery ,

            PaymentStatus =
                PaymentStatus.Pending ,

            DeliveryMethod =
                DeliveryMethod.HomeDelivery ,

            OrderSource =
                OrderSource.Website ,

            CreatedOn =
                new DateTimeOffset(
                    orderDateUtc) ,

            CreatedBy =
                "sales-report-test" ,

            IsDeleted =
                isDeleted ,

            DeletedOn =
                isDeleted
                    ? new DateTimeOffset(
                        orderDateUtc.AddHours(1))
                    : null ,

            DeletedBy =
                isDeleted
                    ? "sales-report-test"
                    : null
        };
    }

    private static OrderStatusHistory CreateDeliveredHistory(
        Order order ,
        DateTime changedAtUtc )
    {
        return new OrderStatusHistory
        {
            OrderId =
                order.Id ,

            PreviousStatus =
                OrderStatus.Shipped ,

            NewStatus =
                OrderStatus.Delivered ,

            ChangedByUserId =
                null ,

            ActorType =
                OrderStatusChangeActorType.System ,

            ChangedAtUtc =
                changedAtUtc
        };
    }

    private static RefundRequest CreateCompletedRefund(
        Order order ,
        string idempotencyKey ,
        decimal refundAmount ,
        DateTime completedAtUtc ,
        bool isDeleted = false )
    {
        return new RefundRequest
        {
            OrderId =
                order.Id ,

            IdempotencyKey =
                idempotencyKey ,

            Status =
                RefundStatus.Completed ,

            CustomerReason =
                "Sales-over-time reporting test refund." ,

            RefundAmount =
                refundAmount ,

            RequestedAt =
                completedAtUtc.AddDays(-1) ,

            CompletedAt =
                completedAtUtc ,

            CreatedOn =
                new DateTimeOffset(
                    completedAtUtc.AddDays(-1)) ,

            CreatedBy =
                "sales-report-test" ,

            IsDeleted =
                isDeleted ,

            DeletedOn =
                isDeleted
                    ? new DateTimeOffset(
                        completedAtUtc)
                    : null ,

            DeletedBy =
                isDeleted
                    ? "sales-report-test"
                    : null
        };
    }
}
