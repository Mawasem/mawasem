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

public sealed class OnlineRefundCompletionTests
{
    private const int DashboardUserId = 51;

    [Fact]
    public async Task
        CompleteAsync_OnlineRefundWithoutPaymobIntegration_IsRejectedWithoutSideEffects()
    {
        await using var database =
            new CheckoutTestDatabase();

        await database.SeedAsync();

        await SeedDashboardUserAsync(
            database);

        var orderInformation =
            await CreateDeliveredOnlineOrderAsync(
                database);

        int refundRequestId;
        int refundRequestItemId;

        await using (
            var dbContext =
                database.CreateContext() )
        {
            var service =
                new RefundRequestService(
                    dbContext ,
                    TimeProvider.System);

            var creationResult =
                await service.CreateAsync(
                    orderInformation.OrderId ,
                    CheckoutTestDatabase.CustomerId ,
                    new CreateRefundRequestRequest
                    {
                        IdempotencyKey =
                            "online-refund-creation-1" ,

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

            refundRequestId =
                creationResult.Response!.Id;

            var refundItem =
                Assert.Single(
                    creationResult.Response.Items);

            refundRequestItemId =
                refundItem.Id;

            var approvalResult =
                await service.ApproveAsync(
                    refundRequestId ,
                    DashboardUserId ,
                    new ApproveRefundRequestRequest
                    {
                        AdminNotes =
                            "Approved pending online refund."
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

            var completionResult =
                await service.CompleteAsync(
                    refundRequestId ,
                    DashboardUserId ,
                    new CompleteRefundRequestRequest
                    {
                        PaymentIdempotencyKey =
                            "online-refund-payment-1" ,

                        ProviderTransactionId =
                            "unverified-paymob-refund-1" ,

                        ProviderReference =
                            "unverified-reference-1" ,

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
                    });

            Assert.False(
                completionResult.Succeeded);

            Assert.Equal(
                RefundRequestErrorCodes.OperationFailed ,
                completionResult.ErrorCode);

            Assert.Equal(
                "Online refund processing is not available " +
                "until the Paymob integration is configured." ,
                completionResult.ErrorMessage);

            Assert.Null(
                completionResult.Response);
        }

        await using var verificationContext =
            database.CreateContext();

        var refundRequest =
            await verificationContext.RefundRequests
                .Include(candidate =>
                    candidate.Items)
                .SingleAsync(candidate =>
                    candidate.Id == refundRequestId);

        Assert.Equal(
            RefundStatus.Approved ,
            refundRequest.Status);

        Assert.Null(
            refundRequest.CompletedAt);

        Assert.Null(
            refundRequest.CompletedByEmployeeId);

        Assert.Null(
            refundRequest.StockRestoredAtUtc);

        var refundItemAfterFailure =
            Assert.Single(
                refundRequest.Items.Where(item =>
                    !item.IsDeleted));

        Assert.Equal(
            0 ,
            refundItemAfterFailure.ReturnedQuantity);

        Assert.Equal(
            0 ,
            refundItemAfterFailure.RestockQuantity);

        var paymentTransactions =
            await verificationContext
                .RefundPaymentTransactions
                .Where(candidate =>
                    candidate.RefundRequestId ==
                    refundRequestId)
                .ToArrayAsync();

        Assert.Empty(
            paymentTransactions);

        var order =
            await verificationContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id ==
                    orderInformation.OrderId);

        Assert.Equal(
            PaymentMethod.Online ,
            order.PaymentMethod);

        Assert.Equal(
            PaymentStatus.Paid ,
            order.PaymentStatus);

        Assert.Equal(
            OrderStatus.RefundRequested ,
            order.OrderStatus);

        var orderItem =
            Assert.Single(
                order.OrderItems.Where(item =>
                    !item.IsDeleted));

        Assert.Equal(
            0 ,
            orderItem.RefundedQuantity);

        var productVariant =
            await verificationContext.ProductVariants
                .SingleAsync(candidate =>
                    candidate.Id ==
                    CheckoutTestDatabase.ProductVariantId);

        // Checkout reduced stock from 10 to 8.
        // The blocked completion must not restore anything.
        Assert.Equal(
            8 ,
            productVariant.StockQuantity);
    }

    private static async Task
        SeedDashboardUserAsync(
            CheckoutTestDatabase database )
    {
        await using var dbContext =
            database.CreateContext();

        dbContext.Users.Add(
            new ApplicationUser
            {
                Id =
                    DashboardUserId ,

                UserName =
                    "online-refund-admin" ,

                NormalizedUserName =
                    "ONLINE-REFUND-ADMIN" ,

                PhoneNumber =
                    "01000000051" ,

                FullNameAr =
                    "مسؤول المرتجعات الإلكترونية" ,

                FullNameEn =
                    "Online Refund Administrator" ,

                SecurityStamp =
                    Guid.NewGuid().ToString()
            });

        await dbContext.SaveChangesAsync();
    }

    private static async Task<OrderInformation>
        CreateDeliveredOnlineOrderAsync(
            CheckoutTestDatabase database )
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

            var checkoutResult =
                await checkoutService.PlaceOrderAsync(
                    CheckoutTestDatabase.CustomerId ,
                    new PlaceOrderRequest
                    {
                        UserAddressId =
                            CheckoutTestDatabase.AddressId ,

                        PaymentMethod =
                            PaymentMethod.CashOnDelivery ,

                        IdempotencyKey =
                            "online-refund-test-order-1"
                    });

            Assert.True(
                checkoutResult.Succeeded);

            Assert.NotNull(
                checkoutResult.Response);

            orderId =
                checkoutResult.Response!.OrderId;
        }

        await using var updateContext =
            database.CreateContext();

        var order =
            await updateContext.Orders
                .Include(candidate =>
                    candidate.OrderItems)
                .SingleAsync(candidate =>
                    candidate.Id == orderId);

        // The payment provider is not integrated yet, so this test
        // converts the persisted order into a successfully paid
        // online order before starting its refund workflow.
        order.PaymentMethod =
            PaymentMethod.Online;

        order.PaymentStatus =
            PaymentStatus.Paid;

        order.OrderStatus =
            OrderStatus.Delivered;

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
}