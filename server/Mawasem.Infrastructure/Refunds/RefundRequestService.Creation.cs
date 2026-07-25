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
    public async Task<
        RefundRequestResult<RefundRequestDetailsResponse>>
        CreateAsync(
            int orderId ,
            int customerUserId ,
            CreateRefundRequestRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( customerUserId <= 0 )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.OrderAccessDenied ,
                    "The customer is not authorized to access this order.");
        }

        var idempotencyKey =
            NormalizeRequiredText(request.IdempotencyKey);

        if ( idempotencyKey is null ||
            idempotencyKey.Length >
            MaximumIdempotencyKeyLength )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidIdempotencyKey ,
                    $"The idempotency key is required and cannot " +
                    $"exceed {MaximumIdempotencyKeyLength} characters.");
        }

        var customerReason =
            NormalizeRequiredText(request.CustomerReason);

        if ( customerReason is null ||
            customerReason.Length >
            MaximumCustomerReasonLength )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.InvalidCustomerReason ,
                    $"The customer reason is required and cannot " +
                    $"exceed {MaximumCustomerReasonLength} characters.");
        }

        var requestedItems =
            request.Items?.ToArray()
            ?? Array.Empty<CreateRefundRequestItemRequest>();

        if ( requestedItems.Length == 0 )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.ItemsRequired ,
                    "At least one refund item is required.");
        }

        var duplicateOrderItemId =
            requestedItems
                .GroupBy(item =>
                    item.OrderItemId)
                .Where(group =>
                    group.Count() > 1)
                .Select(group =>
                    (int?)group.Key)
                .FirstOrDefault();

        if ( duplicateOrderItemId.HasValue )
        {
            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.DuplicateOrderItem ,
                    $"Order item {duplicateOrderItemId.Value} " +
                    "appears more than once in the request.");
        }

        var normalizedItems =
            new List<NormalizedRefundItemInput>(
                requestedItems.Length);

        foreach ( var requestedItem in requestedItems )
        {
            if ( requestedItem.OrderItemId <= 0 )
            {
                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OrderItemNotFound ,
                        "A requested order item was not found.");
            }

            if ( requestedItem.Quantity <= 0 )
            {
                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidQuantity ,
                        $"The refund quantity for order item " +
                        $"{requestedItem.OrderItemId} must be " +
                        "greater than zero.");
            }

            var itemReason =
                NormalizeOptionalText(requestedItem.Reason);

            if ( itemReason?.Length >
                MaximumItemReasonLength )
            {
                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.InvalidCustomerReason ,
                        $"The reason for order item " +
                        $"{requestedItem.OrderItemId} cannot exceed " +
                        $"{MaximumItemReasonLength} characters.");
            }

            normalizedItems.Add(
                new NormalizedRefundItemInput(
                    requestedItem.OrderItemId ,
                    requestedItem.Quantity ,
                    itemReason));
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable ,
                cancellationToken);

        try
        {
            var order =
                await _dbContext.Orders
                    .Include(candidate =>
                        candidate.OrderItems)
                    .Include(candidate =>
                        candidate.RefundRequests)
                    .ThenInclude(refundRequest =>
                        refundRequest.Items)
                    .ThenInclude(refundRequestItem =>
                        refundRequestItem.OrderItem)
                    .AsSplitQuery()
                    .SingleOrDefaultAsync(
                        candidate =>
                            candidate.Id == orderId &&
                            !candidate.IsDeleted ,
                        cancellationToken);

            if ( order is null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OrderNotFound ,
                        "The order was not found.");
            }

            if ( order.UserId != customerUserId )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OrderAccessDenied ,
                        "The customer is not authorized to access this order.");
            }

            var existingRequest =
                order.RefundRequests
                    .FirstOrDefault(candidate =>
                        !candidate.IsDeleted &&
                        string.Equals(
                            candidate.IdempotencyKey ,
                            idempotencyKey ,
                            StringComparison.OrdinalIgnoreCase));

            if ( existingRequest is not null )
            {
                await transaction.CommitAsync(
                    cancellationToken);

                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Success(
                        CreateResponse(existingRequest));
            }

            if ( order.OrderStatus !=
                    OrderStatus.Delivered &&
                order.OrderStatus !=
                    OrderStatus.RefundRequested &&
                order.OrderStatus !=
                    OrderStatus.PartiallyRefunded )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Failure(
                        RefundRequestErrorCodes.OrderNotDelivered ,
                        "A refund can be requested only after " +
                        "the order has been delivered.");
            }

            var orderItemsById =
                order.OrderItems
                    .Where(orderItem =>
                        !orderItem.IsDeleted)
                    .ToDictionary(orderItem =>
                        orderItem.Id);

            var reservedQuantities =
                order.RefundRequests
                    .Where(refundRequest =>
                        !refundRequest.IsDeleted &&
                        refundRequest.Status is
                            RefundStatus.Pending or
                            RefundStatus.Approved)
                    .SelectMany(refundRequest =>
                        refundRequest.Items)
                    .Where(refundRequestItem =>
                        !refundRequestItem.IsDeleted)
                    .GroupBy(refundRequestItem =>
                        refundRequestItem.OrderItemId)
                    .ToDictionary(
                        group =>
                            group.Key ,
                        group =>
                            group.Sum(item =>
                                item.Quantity));

            var now =
                _timeProvider.GetUtcNow();

            var actor =
                GetActor(customerUserId);

            var refundRequest =
                new RefundRequest
                {
                    OrderId =
                        order.Id ,

                    Order =
                        order ,

                    IdempotencyKey =
                        idempotencyKey ,

                    Status =
                        RefundStatus.Pending ,

                    CustomerReason =
                        customerReason ,

                    RefundAmount =
                        0m ,

                    RequestedAt =
                        now.UtcDateTime ,

                    CreatedOn =
                        now ,

                    CreatedBy =
                        actor
                };

            foreach ( var normalizedItem in normalizedItems )
            {
                if ( !orderItemsById.TryGetValue(
                        normalizedItem.OrderItemId ,
                        out var orderItem) )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        RefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes.OrderItemNotFound ,
                            $"Order item " +
                            $"{normalizedItem.OrderItemId} " +
                            "was not found in this order.");
                }

                var reservedQuantity =
                    reservedQuantities.GetValueOrDefault(
                        orderItem.Id);

                var refundableQuantity =
                    orderItem.Quantity -
                    orderItem.RefundedQuantity -
                    reservedQuantity;

                if ( normalizedItem.Quantity >
                    refundableQuantity )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return RefundRequestResult<
                        RefundRequestDetailsResponse>.Failure(
                            RefundRequestErrorCodes
                                .QuantityExceedsRefundable ,
                            $"The requested quantity for order item " +
                            $"{orderItem.Id} exceeds its remaining " +
                            $"refundable quantity of " +
                            $"{Math.Max(refundableQuantity , 0)}.");
                }

                var unitRefundAmount =
                    CalculateUnitRefundAmount(orderItem);

                var totalRefundAmount =
                    CalculateTotalRefundAmount(
                        unitRefundAmount ,
                        normalizedItem.Quantity);

                refundRequest.Items.Add(
                    new RefundRequestItem
                    {
                        OrderItemId =
                            orderItem.Id ,

                        OrderItem =
                            orderItem ,

                        Quantity =
                            normalizedItem.Quantity ,

                        ReturnedQuantity =
                            0 ,

                        RestockQuantity =
                            0 ,

                        Reason =
                            normalizedItem.Reason ,

                        UnitRefundAmount =
                            unitRefundAmount ,

                        TotalRefundAmount =
                            totalRefundAmount ,

                        CreatedOn =
                            now ,

                        CreatedBy =
                            actor
                    });
            }

            refundRequest.RefundAmount =
                decimal.Round(
                    refundRequest.Items.Sum(item =>
                        item.TotalRefundAmount) ,
                    2 ,
                    MidpointRounding.AwayFromZero);

            order.RefundRequests.Add(
                refundRequest);

            if ( order.OrderStatus is
                OrderStatus.Delivered or
                OrderStatus.PartiallyRefunded )
            {
                order.OrderStatus =
                    OrderStatus.RefundRequested;

                MarkModified(
                    order ,
                    now ,
                    actor);
            }

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return RefundRequestResult<
                RefundRequestDetailsResponse>.Success(
                    CreateResponse(refundRequest));
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.ConcurrencyConflict ,
                    "The order or refund information changed while " +
                    "the refund request was being created.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            var replayResponse =
                await TryGetIdempotentReplayAsync(
                    orderId ,
                    customerUserId ,
                    idempotencyKey ,
                    cancellationToken);

            if ( replayResponse is not null )
            {
                return RefundRequestResult<
                    RefundRequestDetailsResponse>.Success(
                        replayResponse);
            }

            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.OperationFailed ,
                    "The refund request could not be created.");
        }
        catch ( OverflowException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return RefundRequestResult<
                RefundRequestDetailsResponse>.Failure(
                    RefundRequestErrorCodes.OperationFailed ,
                    "The refund amount could not be calculated.");
        }
    }
}