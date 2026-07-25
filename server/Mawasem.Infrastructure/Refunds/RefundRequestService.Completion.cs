using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Application.Features.Refunds.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mawasem.Infrastructure.Refunds;

public sealed partial class RefundRequestService
{
    private const int MaximumPaymentIdempotencyKeyLength =
        100;

    private const int MaximumProviderTransactionIdLength =
        200;

    private const int MaximumProviderReferenceLength =
        200;

    public async Task<
        RefundRequestResult<AdminRefundRequestDetailsResponse>>
        CompleteAsync(
            int refundRequestId ,
            int dashboardUserId ,
            CompleteRefundRequestRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( refundRequestId <= 0 )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The refund request identifier must be " +
                    "greater than zero.");
        }

        if ( dashboardUserId <= 0 )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The dashboard user identifier must be " +
                    "greater than zero.");
        }

        var paymentIdempotencyKey =
            NormalizeRequiredText(
                request.PaymentIdempotencyKey);

        if ( paymentIdempotencyKey is null ||
            paymentIdempotencyKey.Length >
            MaximumPaymentIdempotencyKeyLength )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The payment idempotency key is required " +
                    $"and cannot exceed " +
                    $"{MaximumPaymentIdempotencyKeyLength} " +
                    "characters.");
        }

        var providerTransactionId =
            NormalizeOptionalText(
                request.ProviderTransactionId);

        if ( providerTransactionId?.Length >
            MaximumProviderTransactionIdLength )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The provider transaction identifier " +
                    $"cannot exceed " +
                    $"{MaximumProviderTransactionIdLength} " +
                    "characters.");
        }

        var providerReference =
            NormalizeOptionalText(
                request.ProviderReference);

        if ( providerReference?.Length >
            MaximumProviderReferenceLength )
        {
            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidRequest ,
                    "The provider reference cannot exceed " +
                    $"{MaximumProviderReferenceLength} " +
                    "characters.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable ,
                cancellationToken);

        try
        {
            var refundRequest =
                await _dbContext.RefundRequests
                    .Include(candidate =>
                        candidate.Order)
                    .ThenInclude(order =>
                        order.OrderItems)
                    .ThenInclude(orderItem =>
                        orderItem.ProductVariant)
                    .Include(candidate =>
                        candidate.Order)
                    .ThenInclude(order =>
                        order.RefundRequests)
                    .Include(candidate =>
                        candidate.Items)
                    .ThenInclude(item =>
                        item.OrderItem)
                    .ThenInclude(orderItem =>
                        orderItem.ProductVariant)
                    .Include(candidate =>
                        candidate.PaymentTransactions)
                    .AsSplitQuery()
                    .SingleOrDefaultAsync(
                        candidate =>
                            candidate.Id == refundRequestId &&
                            !candidate.IsDeleted &&
                            !candidate.Order.IsDeleted ,
                        cancellationToken);

            if ( refundRequest is null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes
                            .RefundRequestNotFound ,
                        "The refund request was not found.");
            }

            if ( refundRequest.Order.PaymentMethod ==
                PaymentMethod.Online )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OperationFailed ,
                        "Online refund processing is not available " +
                        "until the Paymob integration is configured.");
            }

            var paymentGateway =
                refundRequest.Order.PaymentMethod switch
                {
                    PaymentMethod.CashOnDelivery =>
                        PaymentGateway.None,

                    _ =>
                        (PaymentGateway?)null
                };

            if ( !paymentGateway.HasValue )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OperationFailed ,
                        "The order has an unsupported payment " +
                        "method.");
            }

            if ( providerTransactionId is not null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidRequest ,
                        "A provider transaction identifier must " +
                        "not be supplied for a manually confirmed " +
                        "cash refund. Use the provider reference " +
                        "field for a receipt or manual reference.");
            }

            var existingPaymentTransaction =
                refundRequest.PaymentTransactions
                    .FirstOrDefault(candidate =>
                        !candidate.IsDeleted &&
                        string.Equals(
                            candidate.IdempotencyKey ,
                            paymentIdempotencyKey ,
                            StringComparison.OrdinalIgnoreCase));

            if ( refundRequest.Status ==
                RefundStatus.Completed )
            {
                if ( existingPaymentTransaction is null ||
                    existingPaymentTransaction.Status !=
                    RefundPaymentStatus.Succeeded )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes
                                .InvalidStatusTransition ,
                            "This refund request was already " +
                            "completed using a different payment " +
                            "confirmation.");
                }

                var paymentDetailsMatch =
                    existingPaymentTransaction.PaymentGateway ==
                        paymentGateway.Value &&
                    existingPaymentTransaction.Amount ==
                        refundRequest.RefundAmount &&
                    string.Equals(
                        existingPaymentTransaction
                            .ProviderTransactionId ,
                        providerTransactionId ,
                        StringComparison.Ordinal) &&
                    string.Equals(
                        existingPaymentTransaction
                            .ProviderReference ,
                        providerReference ,
                        StringComparison.Ordinal);

                if ( !paymentDetailsMatch )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes
                                .InvalidStatusTransition ,
                            "The payment idempotency key was " +
                            "previously used with different payment " +
                            "confirmation details.");
                }

                await transaction.CommitAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Success(
                        CreateAdminResponse(refundRequest));
            }

            if ( refundRequest.Status !=
                RefundStatus.Approved )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes
                            .InvalidStatusTransition ,
                        $"A refund request with status " +
                        $"'{refundRequest.Status}' cannot transition " +
                        $"to '{RefundStatus.Completed}'.");
            }

            if ( refundRequest.StockRestoredAtUtc.HasValue )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OperationFailed ,
                        "Stock was previously restored for this " +
                        "refund request, but the request is not " +
                        "marked as completed.");
            }

            if ( refundRequest.RefundAmount <= 0m )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OperationFailed ,
                        "The refund request contains an invalid " +
                        "refund amount.");
            }

            var conflictingPaymentTransaction =
                refundRequest.PaymentTransactions
                    .FirstOrDefault(candidate =>
                        !candidate.IsDeleted &&
                        !string.Equals(
                            candidate.IdempotencyKey ,
                            paymentIdempotencyKey ,
                            StringComparison.OrdinalIgnoreCase) &&
                        candidate.Status is
                            RefundPaymentStatus.Pending or
                            RefundPaymentStatus.Processing or
                            RefundPaymentStatus.Succeeded);

            if ( conflictingPaymentTransaction is not null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes
                            .InvalidStatusTransition ,
                        "This refund request already has another " +
                        "active or successful payment-refund " +
                        "transaction.");
            }

            if ( existingPaymentTransaction is not null )
            {
                if ( existingPaymentTransaction.Status !=
                    RefundPaymentStatus.Succeeded )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes
                                .InvalidStatusTransition ,
                            "The payment idempotency key belongs to " +
                            "a payment-refund transaction that has " +
                            "not succeeded. Use a new key for a new " +
                            "refund attempt.");
                }

                var paymentDetailsMatch =
                    existingPaymentTransaction.PaymentGateway ==
                        paymentGateway.Value &&
                    existingPaymentTransaction.Amount ==
                        refundRequest.RefundAmount &&
                    string.Equals(
                        existingPaymentTransaction
                            .ProviderTransactionId ,
                        providerTransactionId ,
                        StringComparison.Ordinal) &&
                    string.Equals(
                        existingPaymentTransaction
                            .ProviderReference ,
                        providerReference ,
                        StringComparison.Ordinal);

                if ( !paymentDetailsMatch )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes
                                .InvalidStatusTransition ,
                            "The payment idempotency key was " +
                            "previously used with different payment " +
                            "confirmation details.");
                }
            }

            var activeRefundItems =
                refundRequest.Items
                    .Where(item =>
                        !item.IsDeleted)
                    .ToArray();

            if ( activeRefundItems.Length == 0 )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OperationFailed ,
                        "The refund request does not contain any " +
                        "active refund items.");
            }

            var completionItems =
                request.Items?.ToArray()
                ?? Array.Empty<
                    CompleteRefundRequestItemRequest>();

            if ( completionItems.Length == 0 )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidRequest ,
                        "Completion information is required for " +
                        "every refund item.");
            }

            var invalidItemIdentifier =
                completionItems
                    .FirstOrDefault(item =>
                        item.RefundRequestItemId <= 0);

            if ( invalidItemIdentifier is not null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidRequest ,
                        "Every refund-item identifier must be " +
                        "greater than zero.");
            }

            var duplicateItemId =
                completionItems
                    .GroupBy(item =>
                        item.RefundRequestItemId)
                    .Where(group =>
                        group.Count() > 1)
                    .Select(group =>
                        (int?)group.Key)
                    .FirstOrDefault();

            if ( duplicateItemId.HasValue )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidRequest ,
                        $"Refund item {duplicateItemId.Value} " +
                        "appears more than once in the " +
                        "completion request.");
            }

            if ( completionItems.Length !=
                activeRefundItems.Length )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidRequest ,
                        "Completion information must be supplied " +
                        "exactly once for every active refund item.");
            }

            var completionItemsById =
                completionItems.ToDictionary(
                    item =>
                        item.RefundRequestItemId);

            foreach ( var refundItem in activeRefundItems )
            {
                if ( !completionItemsById.TryGetValue(
                        refundItem.Id ,
                        out var completionItem) )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes.InvalidRequest ,
                            $"Completion information for refund " +
                            $"item {refundItem.Id} is missing.");
                }

                if ( completionItem.ReturnedQuantity < 0 ||
                    completionItem.ReturnedQuantity >
                    refundItem.Quantity )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes.InvalidQuantity ,
                            $"Returned quantity for refund item " +
                            $"{refundItem.Id} must be between 0 " +
                            $"and {refundItem.Quantity}.");
                }

                if ( completionItem.RestockQuantity < 0 ||
                    completionItem.RestockQuantity >
                    completionItem.ReturnedQuantity )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes.InvalidQuantity ,
                            $"Restock quantity for refund item " +
                            $"{refundItem.Id} must be between 0 " +
                            "and its returned quantity.");
                }
            }

            var activeRefundItemIds =
                activeRefundItems
                    .Select(item =>
                        item.Id)
                    .ToHashSet();

            var unrelatedCompletionItem =
                completionItems
                    .FirstOrDefault(item =>
                        !activeRefundItemIds.Contains(
                            item.RefundRequestItemId));

            if ( unrelatedCompletionItem is not null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidRequest ,
                        $"Refund item " +
                        $"{unrelatedCompletionItem.RefundRequestItemId} " +
                        "does not belong to this refund request.");
            }

            var now =
                _timeProvider.GetUtcNow();

            var actor =
                GetActor(dashboardUserId);

            var totalRestockQuantity =
                0;

            foreach ( var refundItem in activeRefundItems )
            {
                var completionItem =
                    completionItemsById[refundItem.Id];

                if ( refundItem.Quantity <= 0 )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes.OperationFailed ,
                            $"Refund item {refundItem.Id} contains " +
                            "an invalid approved quantity.");
                }

                var orderItem =
                    refundItem.OrderItem;

                if ( orderItem.IsDeleted ||
                    orderItem.OrderId !=
                    refundRequest.OrderId )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes.OperationFailed ,
                            $"Order item {orderItem.Id} is not an " +
                            "active item of this order.");
                }

                if ( orderItem.RefundedQuantity < 0 ||
                    orderItem.RefundedQuantity >
                    orderItem.Quantity )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes.OperationFailed ,
                            $"Order item {orderItem.Id} contains an " +
                            "invalid refunded quantity.");
                }

                var remainingRefundableQuantity =
                    orderItem.Quantity -
                    orderItem.RefundedQuantity;

                if ( refundItem.Quantity >
                    remainingRefundableQuantity )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        AdminRefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes
                                .QuantityExceedsRefundable ,
                            $"Refund item {refundItem.Id} exceeds " +
                            $"the remaining refundable quantity of " +
                            $"{remainingRefundableQuantity}.");
                }

                refundItem.ReturnedQuantity =
                    completionItem.ReturnedQuantity;

                refundItem.RestockQuantity =
                    completionItem.RestockQuantity;

                MarkModified(
                    refundItem ,
                    now ,
                    actor);

                orderItem.RefundedQuantity =
                    checked(
                        orderItem.RefundedQuantity +
                        refundItem.Quantity);

                MarkModified(
                    orderItem ,
                    now ,
                    actor);

                if ( completionItem.RestockQuantity <= 0 )
                {
                    continue;
                }

                var productVariant =
                    orderItem.ProductVariant;

                productVariant.StockQuantity =
                    checked(
                        productVariant.StockQuantity +
                        completionItem.RestockQuantity);

                MarkModified(
                    productVariant ,
                    now ,
                    actor);

                totalRestockQuantity =
                    checked(
                        totalRestockQuantity +
                        completionItem.RestockQuantity);
            }

            if ( existingPaymentTransaction is null )
            {
                refundRequest.PaymentTransactions.Add(
                    new RefundPaymentTransaction
                    {
                        RefundRequestId =
                            refundRequest.Id ,

                        RefundRequest =
                            refundRequest ,

                        PaymentGateway =
                            paymentGateway.Value ,

                        Status =
                            RefundPaymentStatus.Succeeded ,

                        Amount =
                            refundRequest.RefundAmount ,

                        IdempotencyKey =
                            paymentIdempotencyKey ,

                        ProviderTransactionId =
                            providerTransactionId ,

                        ProviderReference =
                            providerReference ,

                        RequestedAt =
                            now.UtcDateTime ,

                        CompletedAt =
                            now.UtcDateTime ,

                        InitiatedByEmployeeId =
                            dashboardUserId ,

                        CompletedByEmployeeId =
                            dashboardUserId ,

                        CreatedOn =
                            now ,

                        CreatedBy =
                            actor
                    });
            }

            refundRequest.Status =
                RefundStatus.Completed;

            refundRequest.CompletedAt =
                now.UtcDateTime;

            refundRequest.CompletedByEmployeeId =
                dashboardUserId;

            refundRequest.StockRestoredAtUtc =
                totalRestockQuantity > 0
                    ? now.UtcDateTime
                    : null;

            MarkModified(
                refundRequest ,
                now ,
                actor);

            UpdateOrderAndPaymentStatusAfterCompletion(
                refundRequest ,
                now ,
                actor);

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Success(
                    CreateAdminResponse(refundRequest));
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            var replayResponse =
                await TryGetCompletionReplayAsync(
                    refundRequestId ,
                    paymentIdempotencyKey ,
                    cancellationToken);

            if ( replayResponse is not null )
            {
                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Success(
                        replayResponse);
            }

            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.ConcurrencyConflict ,
                    "The refund request, order, payment " +
                    "confirmation, or product stock changed while " +
                    "the refund was being completed.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            var replayResponse =
                await TryGetCompletionReplayAsync(
                    refundRequestId ,
                    paymentIdempotencyKey ,
                    cancellationToken);

            if ( replayResponse is not null )
            {
                return RefundRequestResult<
                    AdminRefundRequestDetailsResponse>.Success(
                        replayResponse);
            }

            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.OperationFailed ,
                    "The completed refund and its payment " +
                    "confirmation could not be saved.");
        }
        catch ( OverflowException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return RefundRequestResult<
                AdminRefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.OperationFailed ,
                    "The refunded quantity or restored stock " +
                    "exceeded the supported range.");
        }
    }

    private async Task<AdminRefundRequestDetailsResponse?>
        TryGetCompletionReplayAsync(
            int refundRequestId ,
            string paymentIdempotencyKey ,
            CancellationToken cancellationToken )
    {
        var refundRequest =
            await _dbContext.RefundRequests
                .AsNoTracking()
                .Include(candidate =>
                    candidate.Order)
                .Include(candidate =>
                    candidate.Items)
                .ThenInclude(item =>
                    item.OrderItem)
                .Include(candidate =>
                    candidate.PaymentTransactions)
                .AsSplitQuery()
                .SingleOrDefaultAsync(
                    candidate =>
                        candidate.Id == refundRequestId &&
                        !candidate.IsDeleted &&
                        !candidate.Order.IsDeleted &&
                        candidate.Status ==
                            RefundStatus.Completed &&
                        candidate.PaymentTransactions.Any(
                            paymentTransaction =>
                                !paymentTransaction.IsDeleted &&
                                paymentTransaction.Status ==
                                    RefundPaymentStatus.Succeeded &&
                                paymentTransaction.IdempotencyKey ==
                                    paymentIdempotencyKey) ,
                    cancellationToken);

        return refundRequest is null
            ? null
            : CreateAdminResponse(refundRequest);
    }

    private static void
        UpdateOrderAndPaymentStatusAfterCompletion(
            RefundRequest completedRequest ,
            DateTimeOffset now ,
            string actor )
    {
        var order =
            completedRequest.Order;

        var activeOrderItems =
            order.OrderItems
                .Where(item =>
                    !item.IsDeleted)
                .ToArray();

        var isFullyRefunded =
            activeOrderItems.Length > 0 &&
            activeOrderItems.All(item =>
                item.RefundedQuantity >=
                item.Quantity);

        var hasAnotherActiveRefundRequest =
            order.RefundRequests.Any(
                candidate =>
                    candidate.Id != completedRequest.Id &&
                    !candidate.IsDeleted &&
                    candidate.Status is
                        RefundStatus.Pending or
                        RefundStatus.Approved);

        var targetOrderStatus =
            isFullyRefunded
                ? OrderStatus.Refunded
                : hasAnotherActiveRefundRequest
                    ? OrderStatus.RefundRequested
                    : OrderStatus.PartiallyRefunded;

        var completedRefundAmount =
            order.RefundRequests
                .Where(candidate =>
                    !candidate.IsDeleted &&
                    candidate.Status ==
                    RefundStatus.Completed)
                .Sum(candidate =>
                    candidate.RefundAmount);

        var targetPaymentStatus =
            order.PaymentStatus;

        if ( completedRefundAmount > 0m )
        {
            targetPaymentStatus =
                isFullyRefunded &&
                order.DeliveryFee == 0m
                    ? PaymentStatus.Refunded
                    : PaymentStatus.PartiallyRefunded;
        }

        var orderStatusChanged =
            order.OrderStatus !=
            targetOrderStatus;

        var paymentStatusChanged =
            order.PaymentStatus !=
            targetPaymentStatus;

        if ( !orderStatusChanged &&
            !paymentStatusChanged )
        {
            return;
        }

        order.OrderStatus =
            targetOrderStatus;

        order.PaymentStatus =
            targetPaymentStatus;

        MarkModified(
            order ,
            now ,
            actor);
    }
}