using Mawasem.Application.Features.Reports.Contracts.Requests;
using Mawasem.Application.Features.Reports.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Reports;
using Mawasem.Tests.Checkout;

namespace Mawasem.Tests.Reports;

public sealed class BusinessDashboardReportTests
{
    private static readonly DateTime BaseTimeUtc =
        new(
            2026 ,
            7 ,
            10 ,
            0 ,
            0 ,
            0 ,
            DateTimeKind.Utc);

    [Fact]
    public async Task
        GetBusinessDashboardAsync_MixedData_ReturnsExpectedMetrics()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedDashboardScenarioAsync(
            database);

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetBusinessDashboardAsync(
                    new GetBusinessDashboardRequest
                    {
                        FromDateUtc =
                            BaseTimeUtc ,

                        ToDateUtc =
                            BaseTimeUtc.AddDays(10)
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        var response =
            result.Response!;

        Assert.Equal(
            BaseTimeUtc ,
            response.FromDateUtc);

        Assert.Equal(
            BaseTimeUtc.AddDays(10) ,
            response.ToDateUtc);

        Assert.Equal(
            5 ,
            response.TotalOrders);

        Assert.Equal(
            2 ,
            response.DeliveredOrders);

        Assert.Equal(
            1 ,
            response.PendingFulfillmentOrders);

        Assert.Equal(
            1 ,
            response.CancelledOrders);

        Assert.Equal(
            1 ,
            response.RejectedOrders);

        Assert.Equal(
            300m ,
            response.GrossSales);

        Assert.Equal(
            40m ,
            response.CompletedRefundAmount);

        Assert.Equal(
            260m ,
            response.NetRevenue);

        Assert.Equal(
            150m ,
            response.AverageOrderValue);

        Assert.Equal(
            Enum.GetValues<OrderStatus>().Length ,
            response.OrderStatusCounts.Count);

        AssertStatusCount(
            response.OrderStatusCounts ,
            OrderStatus.Pending ,
            1);

        AssertStatusCount(
            response.OrderStatusCounts ,
            OrderStatus.Delivered ,
            2);

        AssertStatusCount(
            response.OrderStatusCounts ,
            OrderStatus.Cancelled ,
            1);

        AssertStatusCount(
            response.OrderStatusCounts ,
            OrderStatus.Rejected ,
            1);

        AssertStatusCount(
            response.OrderStatusCounts ,
            OrderStatus.Confirmed ,
            0);
    }

    [Fact]
    public async Task
        GetBusinessDashboardAsync_NoMatchingData_ReturnsZeros()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetBusinessDashboardAsync(
                    new GetBusinessDashboardRequest
                    {
                        FromDateUtc =
                            new DateTime(
                                2050 ,
                                1 ,
                                1 ,
                                0 ,
                                0 ,
                                0 ,
                                DateTimeKind.Utc) ,

                        ToDateUtc =
                            new DateTime(
                                2050 ,
                                1 ,
                                31 ,
                                23 ,
                                59 ,
                                59 ,
                                DateTimeKind.Utc)
                    });

        Assert.True(
            result.Succeeded ,
            $"{result.ErrorCode}: {result.ErrorMessage}");

        Assert.NotNull(
            result.Response);

        var response =
            result.Response!;

        Assert.Equal(
            0 ,
            response.TotalOrders);

        Assert.Equal(
            0 ,
            response.DeliveredOrders);

        Assert.Equal(
            0 ,
            response.PendingFulfillmentOrders);

        Assert.Equal(
            0m ,
            response.GrossSales);

        Assert.Equal(
            0m ,
            response.CompletedRefundAmount);

        Assert.Equal(
            0m ,
            response.NetRevenue);

        Assert.Equal(
            0m ,
            response.AverageOrderValue);

        Assert.All(
            response.OrderStatusCounts ,
            item =>
                Assert.Equal(
                    0 ,
                    item.Count));
    }

    [Fact]
    public async Task
        GetBusinessDashboardAsync_InvalidDateRange_ReturnsFailure()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await using var dbContext =
            database.CreateContext();

        var result =
            await new ReportService(dbContext)
                .GetBusinessDashboardAsync(
                    new GetBusinessDashboardRequest
                    {
                        FromDateUtc =
                            BaseTimeUtc.AddDays(1) ,

                        ToDateUtc =
                            BaseTimeUtc
                    });

        Assert.False(
            result.Succeeded);

        Assert.Equal(
            ReportErrorCodes.InvalidDateRange ,
            result.ErrorCode);

        Assert.Null(
            result.Response);
    }

    private static async Task SeedDashboardScenarioAsync(
        CheckoutTestDatabase database )
    {
        await using var dbContext =
            database.CreateContext();

        var deliveredOutsideOrderRange =
            CreateOrder(
                "DASH-001" ,
                OrderStatus.Delivered ,
                100m ,
                BaseTimeUtc.AddDays(-5));

        var deliveredInsideRange =
            CreateOrder(
                "DASH-002" ,
                OrderStatus.Delivered ,
                200m ,
                BaseTimeUtc.AddDays(1));

        var pendingInsideRange =
            CreateOrder(
                "DASH-003" ,
                OrderStatus.Pending ,
                50m ,
                BaseTimeUtc.AddDays(2));

        var cancelledInsideRange =
            CreateOrder(
                "DASH-004" ,
                OrderStatus.Cancelled ,
                75m ,
                BaseTimeUtc.AddDays(3));

        var rejectedInsideRange =
            CreateOrder(
                "DASH-005" ,
                OrderStatus.Rejected ,
                80m ,
                BaseTimeUtc.AddDays(4));

        var deliveredOutsideSalesRange =
            CreateOrder(
                "DASH-006" ,
                OrderStatus.Delivered ,
                300m ,
                BaseTimeUtc.AddDays(5));

        var deletedDeliveredOrder =
            CreateOrder(
                "DASH-007" ,
                OrderStatus.Delivered ,
                400m ,
                BaseTimeUtc.AddDays(6) ,
                isDeleted: true);

        dbContext.Orders.AddRange(
            deliveredOutsideOrderRange ,
            deliveredInsideRange ,
            pendingInsideRange ,
            cancelledInsideRange ,
            rejectedInsideRange ,
            deliveredOutsideSalesRange ,
            deletedDeliveredOrder);

        await dbContext.SaveChangesAsync();

        dbContext.OrderStatusHistories.AddRange(
            CreateDeliveredHistory(
                deliveredOutsideOrderRange ,
                BaseTimeUtc.AddDays(2)) ,

            CreateDeliveredHistory(
                deliveredInsideRange ,
                BaseTimeUtc.AddDays(3)) ,

            CreateDeliveredHistory(
                deliveredOutsideSalesRange ,
                BaseTimeUtc.AddDays(11)) ,

            CreateDeliveredHistory(
                deletedDeliveredOrder ,
                BaseTimeUtc.AddDays(4)));

        dbContext.RefundRequests.AddRange(
            CreateCompletedRefund(
                deliveredInsideRange ,
                "dashboard-refund-1" ,
                40m ,
                BaseTimeUtc.AddDays(5)) ,

            CreateCompletedRefund(
                deliveredOutsideOrderRange ,
                "dashboard-refund-2" ,
                25m ,
                BaseTimeUtc.AddDays(11)) ,

            CreateCompletedRefund(
                deliveredInsideRange ,
                "dashboard-refund-3" ,
                10m ,
                BaseTimeUtc.AddDays(6) ,
                isDeleted: true) ,

            CreateCompletedRefund(
                deletedDeliveredOrder ,
                "dashboard-refund-4" ,
                20m ,
                BaseTimeUtc.AddDays(7)));

        await dbContext.SaveChangesAsync();
    }

    private static Order CreateOrder(
        string orderNumber ,
        OrderStatus status ,
        decimal totalAmount ,
        DateTime orderDateUtc ,
        bool isDeleted = false )
    {
        return new Order
        {
            UserId =
                CheckoutTestDatabase.CustomerId ,

            CustomerNameAr =
                "عميل تقارير الأعمال" ,

            CustomerNameEn =
                "Business Report Customer" ,

            CustomerPhone =
                "01000000001" ,

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
                status ,

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
                "business-report-test" ,

            IsDeleted =
                isDeleted ,

            DeletedOn =
                isDeleted
                    ? new DateTimeOffset(
                        orderDateUtc.AddHours(1))
                    : null ,

            DeletedBy =
                isDeleted
                    ? "business-report-test"
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
                "Dashboard reporting test refund." ,

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
                "business-report-test" ,

            IsDeleted =
                isDeleted ,

            DeletedOn =
                isDeleted
                    ? new DateTimeOffset(
                        completedAtUtc)
                    : null ,

            DeletedBy =
                isDeleted
                    ? "business-report-test"
                    : null
        };
    }

    private static void AssertStatusCount(
        IReadOnlyList<
            Mawasem.Application.Features.Reports.Contracts.Responses
                .OrderStatusCountResponse> items ,
        OrderStatus status ,
        int expectedCount )
    {
        var item =
            Assert.Single(items, candidate =>
                    candidate.Status ==
                    status);

        Assert.Equal(
            expectedCount ,
            item.Count);
    }
}
