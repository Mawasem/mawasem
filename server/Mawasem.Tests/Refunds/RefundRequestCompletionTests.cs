using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Identity;
using Mawasem.Infrastructure.Checkout;
using Mawasem.Infrastructure.Refunds;
using Mawasem.Tests.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Tests.Refunds;

public sealed class RefundRequestCompletionTests
{
    private const int DashboardUserId = 50;

    [Fact]
    public async Task
        CompleteAsync_CashPartialRefund_RecordsPaymentAndRestoresOnlyRestockQuantity()
    {
        await using var database =
            new CheckoutTestDatabase();

        var scenario =
            await CreateApprovedRefundScenarioAsync(
                database ,
                "refund-test-order-1" ,
                "partial-cash-refund-creation-1");

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                CreateRefundService(
                    dbContext);

            var completionResult =
                await service.CompleteAsync(
                    scenario.RefundRequestId ,
                    DashboardUserId ,
                    CreateCashCompletionRequest(
                        scenario.RefundRequestItemId ,
                        "cash-refund-payment-1" ,
                        "cash-receipt-1"));

            Assert.True(
                completionResult.Succeeded ,
                $"{completionResult.ErrorCode}: " +
                $"{completionResult.ErrorMessage}");

            Assert.NotNull(
                completionResult.Response);

            Assert.Equal(
                RefundStatus.Completed ,
                completionResult.Response!.Status);

            Assert.Equal(
                100m ,
                completionResult.Response.RefundAmount);

            var completedItem =
                Assert.Single(
                    completionResult.Response.Items);

            Assert.Equal(
                1 ,
                completedItem.Quantity);

            Assert.Equal(
                1 ,
                completedItem.ReturnedQuantity);

            Assert.Equal(
                1 ,
                completedItem.RestockQuantity);

            Assert.NotNull(
                completionResult.Response.CompletedAt);

            Assert.NotNull(
                completionResult.Response
                    .StockRestoredAtUtc);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id ==
                    scenario.OrderId);

        Assert.Equal(
            OrderStatus.PartiallyRefunded ,
            order.OrderStatus);

        Assert.Equal(
            PaymentStatus.PartiallyRefunded ,
            order.PaymentStatus);

        Assert.Equal(
            200m ,
            order.SubTotal);

        Assert.Equal(
            25m ,
            order.DeliveryFee);

        Assert.Equal(
            225m ,
            order.TotalAmount);

        var orderItem =
            Assert.Single(
                order.OrderItems.Where(item =>
                    !item.IsDeleted));

        Assert.Equal(
            2 ,
            orderItem.Quantity);

        Assert.Equal(
            1 ,
            orderItem.RefundedQuantity);

        var productVariant =
            await verificationContext.ProductVariants
                .SingleAsync(candidate =>
                    candidate.Id ==
                    CheckoutTestDatabase.ProductVariantId);

        // Checkout reduced stock from 10 to 8.
        // Completion restores only one sellable unit.
        Assert.Equal(
            9 ,
            productVariant.StockQuantity);

        var refundRequest =
            await verificationContext.RefundRequests
                .Include(candidate =>
                    candidate.Items)
                .SingleAsync(candidate =>
                    candidate.Id ==
                    scenario.RefundRequestId);

        Assert.Equal(
            RefundStatus.Completed ,
            refundRequest.Status);

        Assert.Equal(
            100m ,
            refundRequest.RefundAmount);

        Assert.Equal(
            DashboardUserId ,
            refundRequest.ReviewedByEmployeeId);

        Assert.Equal(
            DashboardUserId ,
            refundRequest.CompletedByEmployeeId);

        var refundItem =
            Assert.Single(
                refundRequest.Items.Where(item =>
                    !item.IsDeleted));

        Assert.Equal(
            1 ,
            refundItem.Quantity);

        Assert.Equal(
            1 ,
            refundItem.ReturnedQuantity);

        Assert.Equal(
            1 ,
            refundItem.RestockQuantity);

        var paymentTransaction =
            await verificationContext
                .RefundPaymentTransactions
                .SingleAsync(candidate =>
                    candidate.RefundRequestId ==
                    scenario.RefundRequestId);

        Assert.Equal(
            PaymentGateway.None ,
            paymentTransaction.PaymentGateway);

        Assert.Equal(
            RefundPaymentStatus.Succeeded ,
            paymentTransaction.Status);

        Assert.Equal(
            100m ,
            paymentTransaction.Amount);

        Assert.Equal(
            "cash-refund-payment-1" ,
            paymentTransaction.IdempotencyKey);

        Assert.Null(
            paymentTransaction.ProviderTransactionId);

        Assert.Equal(
            "cash-receipt-1" ,
            paymentTransaction.ProviderReference);

        Assert.Equal(
            DashboardUserId ,
            paymentTransaction.InitiatedByEmployeeId);

        Assert.Equal(
            DashboardUserId ,
            paymentTransaction.CompletedByEmployeeId);

        Assert.NotNull(
            paymentTransaction.CompletedAt);
    }

    [Fact]
    public async Task
        CompleteAsync_RepeatedIdenticalCashCompletion_DoesNotApplyRefundTwice()
    {
        await using var database =
            new CheckoutTestDatabase();

        var scenario =
            await CreateApprovedRefundScenarioAsync(
                database ,
                "refund-idempotent-order-1" ,
                "refund-idempotent-creation-1");

        var completionRequest =
            CreateCashCompletionRequest(
                scenario.RefundRequestItemId ,
                "cash-refund-idempotency-1" ,
                "cash-idempotency-receipt-1");

        await using (
            var firstContext =
                database.CreateContext() )
        {
            var service =
                CreateRefundService(
                    firstContext);

            var firstResult =
                await service.CompleteAsync(
                    scenario.RefundRequestId ,
                    DashboardUserId ,
                    completionRequest);

            Assert.True(
                firstResult.Succeeded ,
                $"{firstResult.ErrorCode}: " +
                $"{firstResult.ErrorMessage}");

            Assert.NotNull(
                firstResult.Response);

            Assert.Equal(
                RefundStatus.Completed ,
                firstResult.Response!.Status);
        }

        await using (
            var secondContext =
                database.CreateContext() )
        {
            var service =
                CreateRefundService(
                    secondContext);

            var secondResult =
                await service.CompleteAsync(
                    scenario.RefundRequestId ,
                    DashboardUserId ,
                    completionRequest);

            Assert.True(
                secondResult.Succeeded ,
                $"{secondResult.ErrorCode}: " +
                $"{secondResult.ErrorMessage}");

            Assert.NotNull(
                secondResult.Response);

            Assert.Equal(
                RefundStatus.Completed ,
                secondResult.Response!.Status);

            var replayedItem =
                Assert.Single(
                    secondResult.Response.Items);

            Assert.Equal(
                1 ,
                replayedItem.Quantity);

            Assert.Equal(
                1 ,
                replayedItem.ReturnedQuantity);

            Assert.Equal(
                1 ,
                replayedItem.RestockQuantity);
        }

        await using var verificationContext =
            database.CreateContext();

        var order =
            await verificationContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id ==
                    scenario.OrderId);

        Assert.Equal(
            OrderStatus.PartiallyRefunded ,
            order.OrderStatus);

        Assert.Equal(
            PaymentStatus.PartiallyRefunded ,
            order.PaymentStatus);

        var orderItem =
            Assert.Single(
                order.OrderItems.Where(item =>
                    !item.IsDeleted));

        // The approved quantity must be applied only once.
        Assert.Equal(
            1 ,
            orderItem.RefundedQuantity);

        var productVariant =
            await verificationContext.ProductVariants
                .SingleAsync(candidate =>
                    candidate.Id ==
                    CheckoutTestDatabase.ProductVariantId);

        // Checkout: 10 -> 8.
        // First completion: 8 -> 9.
        // Replay must leave it at 9.
        Assert.Equal(
            9 ,
            productVariant.StockQuantity);

        var paymentTransactions =
            await verificationContext
                .RefundPaymentTransactions
                .Where(candidate =>
                    candidate.RefundRequestId ==
                    scenario.RefundRequestId &&
                    !candidate.IsDeleted)
                .ToArrayAsync();

        var paymentTransaction =
            Assert.Single(
                paymentTransactions);

        Assert.Equal(
            RefundPaymentStatus.Succeeded ,
            paymentTransaction.Status);

        Assert.Equal(
            "cash-refund-idempotency-1" ,
            paymentTransaction.IdempotencyKey);

        Assert.Equal(
            "cash-idempotency-receipt-1" ,
            paymentTransaction.ProviderReference);

        var refundRequest =
            await verificationContext.RefundRequests
                .Include(candidate =>
                    candidate.Items)
                .SingleAsync(candidate =>
                    candidate.Id ==
                    scenario.RefundRequestId);

        Assert.Equal(
            RefundStatus.Completed ,
            refundRequest.Status);

        var refundItem =
            Assert.Single(
                refundRequest.Items.Where(item =>
                    !item.IsDeleted));

        Assert.Equal(
            1 ,
            refundItem.ReturnedQuantity);

        Assert.Equal(
            1 ,
            refundItem.RestockQuantity);
    }

    [Fact]
    public async Task
        CompleteAsync_SamePaymentKeyWithDifferentDetails_IsRejected()
    {
        await using var database =
            new CheckoutTestDatabase();

        var scenario =
            await CreateApprovedRefundScenarioAsync(
                database ,
                "refund-conflict-order-1" ,
                "refund-conflict-creation-1");

        const string paymentIdempotencyKey =
            "cash-refund-conflict-1";

        await using (
            var firstContext =
                database.CreateContext() )
        {
            var service =
                CreateRefundService(
                    firstContext);

            var firstResult =
                await service.CompleteAsync(
                    scenario.RefundRequestId ,
                    DashboardUserId ,
                    CreateCashCompletionRequest(
                        scenario.RefundRequestItemId ,
                        paymentIdempotencyKey ,
                        "original-cash-receipt"));

            Assert.True(
                firstResult.Succeeded ,
                $"{firstResult.ErrorCode}: " +
                $"{firstResult.ErrorMessage}");

            Assert.NotNull(
                firstResult.Response);

            Assert.Equal(
                RefundStatus.Completed ,
                firstResult.Response!.Status);
        }

        await using (
            var secondContext =
                database.CreateContext() )
        {
            var service =
                CreateRefundService(
                    secondContext);

            var conflictingResult =
                await service.CompleteAsync(
                    scenario.RefundRequestId ,
                    DashboardUserId ,
                    CreateCashCompletionRequest(
                        scenario.RefundRequestItemId ,
                        paymentIdempotencyKey ,
                        "different-cash-receipt"));

            Assert.False(
                conflictingResult.Succeeded);

            Assert.Equal(
                RefundRequestErrorCodes
                    .InvalidStatusTransition ,
                conflictingResult.ErrorCode);

            Assert.Null(
                conflictingResult.Response);
        }

        await using var verificationContext =
            database.CreateContext();

        var paymentTransactions =
            await verificationContext
                .RefundPaymentTransactions
                .Where(candidate =>
                    candidate.RefundRequestId ==
                    scenario.RefundRequestId &&
                    !candidate.IsDeleted)
                .ToArrayAsync();

        var paymentTransaction =
            Assert.Single(
                paymentTransactions);

        Assert.Equal(
            paymentIdempotencyKey ,
            paymentTransaction.IdempotencyKey);

        Assert.Equal(
            "original-cash-receipt" ,
            paymentTransaction.ProviderReference);

        var order =
            await verificationContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id ==
                    scenario.OrderId);

        var orderItem =
            Assert.Single(
                order.OrderItems.Where(item =>
                    !item.IsDeleted));

        Assert.Equal(
            1 ,
            orderItem.RefundedQuantity);

        var productVariant =
            await verificationContext.ProductVariants
                .SingleAsync(candidate =>
                    candidate.Id ==
                    CheckoutTestDatabase.ProductVariantId);

        Assert.Equal(
            9 ,
            productVariant.StockQuantity);
    }

    private static async Task<RefundScenario>
        CreateApprovedRefundScenarioAsync(
            CheckoutTestDatabase database ,
            string orderIdempotencyKey ,
            string refundIdempotencyKey )
    {
        await database.SeedAsync();

        await SeedDashboardUserAsync(
            database);

        var orderInformation =
            await CreateDeliveredOrderAsync(
                database ,
                orderIdempotencyKey);

        await using var dbContext =
            database.CreateContext();

        var service =
            CreateRefundService(
                dbContext);

        var creationResult =
            await service.CreateAsync(
                orderInformation.OrderId ,
                CheckoutTestDatabase.CustomerId ,
                new CreateRefundRequestRequest
                {
                    IdempotencyKey =
                        refundIdempotencyKey ,

                    CustomerReason =
                        "One unit was damaged." ,

                    Items =
                        new[]
                        {
                            new CreateRefundRequestItemRequest
                            {
                                OrderItemId =
                                    orderInformation.OrderItemId ,

                                Quantity =
                                    1 ,

                                Reason =
                                    "Damaged unit"
                            }
                        }
                });

        Assert.True(
            creationResult.Succeeded ,
            $"{creationResult.ErrorCode}: " +
            $"{creationResult.ErrorMessage}");

        Assert.NotNull(
            creationResult.Response);

        Assert.Equal(
            100m ,
            creationResult.Response!.RefundAmount);

        Assert.Equal(
            RefundStatus.Pending ,
            creationResult.Response.Status);

        var createdItem =
            Assert.Single(
                creationResult.Response.Items);

        var approvalResult =
            await service.ApproveAsync(
                creationResult.Response.Id ,
                DashboardUserId ,
                new ApproveRefundRequestRequest
                {
                    AdminNotes =
                        "Approved after inspection."
                });

        Assert.True(
            approvalResult.Succeeded ,
            $"{approvalResult.ErrorCode}: " +
            $"{approvalResult.ErrorMessage}");

        Assert.NotNull(
            approvalResult.Response);

        Assert.Equal(
            RefundStatus.Approved ,
            approvalResult.Response!.Status);

        return new RefundScenario(
            orderInformation.OrderId ,
            creationResult.Response.Id ,
            createdItem.Id);
    }

    private static CompleteRefundRequestRequest
        CreateCashCompletionRequest(
            int refundRequestItemId ,
            string paymentIdempotencyKey ,
            string providerReference )
    {
        return new CompleteRefundRequestRequest
        {
            PaymentIdempotencyKey =
                paymentIdempotencyKey ,

            ProviderReference =
                providerReference ,

            Items =
                new[]
                {
                    new CompleteRefundRequestItemRequest
                    {
                        RefundRequestItemId =
                            refundRequestItemId ,

                        ReturnedQuantity =
                            1 ,

                        RestockQuantity =
                            1
                    }
                }
        };
    }

    private static RefundRequestService
        CreateRefundService(
            CheckoutTestDbContext dbContext )
    {
        return new RefundRequestService(
            dbContext ,
            TimeProvider.System);
    }

    private static async Task
        SeedDashboardUserAsync(
            CheckoutTestDatabase database )
    {
        await using var dbContext =
            database.CreateContext();

        if ( await dbContext.Users
            .AnyAsync(
                candidate =>
                    candidate.Id ==
                    DashboardUserId) )
        {
            return;
        }
        dbContext.Users.Add(
            new ApplicationUser
            {
                Id =
                    DashboardUserId ,

                UserName =
                    "refund-admin" ,

                NormalizedUserName =
                    "REFUND-ADMIN" ,

                PhoneNumber =
                    "01000000050" ,

                FullNameAr =
                    "مسؤول المرتجعات" ,

                FullNameEn =
                    "Refund Administrator" ,

                SecurityStamp =
                    Guid.NewGuid().ToString()
            });

        await dbContext.SaveChangesAsync();
    }

    private static async Task<OrderInformation>
        CreateDeliveredOrderAsync(
            CheckoutTestDatabase database ,
            string idempotencyKey )
    {
        int orderId;

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var checkoutService =
                new CheckoutService(
                    dbContext ,
                    TimeProvider.System);

            var result =
                await checkoutService.PlaceOrderAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new PlaceOrderRequest
                    {
                        UserAddressId =
                            CheckoutTestDatabase.AddressId ,

                        PaymentMethod =
                            PaymentMethod.CashOnDelivery ,

                        IdempotencyKey =
                            idempotencyKey
                    });

            Assert.True(
                result.Succeeded);

            Assert.NotNull(
                result.Response);

            orderId =
                result.Response!.OrderId;
        }

        await using var updateContext =
            database.CreateContext();

        var order =
            await updateContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        order.OrderStatus =
            OrderStatus.Delivered;

        order.PaymentStatus =
            PaymentStatus.Paid;

        await updateContext.SaveChangesAsync();

        var orderItem =
            Assert.Single(
                order.OrderItems.Where(item =>
                    !item.IsDeleted));

        return new OrderInformation(
            order.Id ,
            orderItem.Id);
    }

    private sealed record OrderInformation(
        int OrderId ,
        int OrderItemId );

    private sealed record RefundScenario(
        int OrderId ,
        int RefundRequestId ,
        int RefundRequestItemId );
}
