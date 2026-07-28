using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Contracts.Responses;
using Mawasem.Application.Features.StoreOrders.Interfaces;
using Mawasem.Application.Features.StoreOrders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Domain.Orders;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace Mawasem.Infrastructure.StoreOrders;

public sealed class StorePickupCollectionService
    : IStorePickupCollectionService
{
    private const int MaximumPaymentReferenceLength = 200;

    private const int MaximumNotesLength = 500;

    private readonly MawasemDbContext _dbContext;

    private readonly TimeProvider _timeProvider;

    public StorePickupCollectionService(
        MawasemDbContext dbContext ,
        TimeProvider timeProvider )
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task<StoreOrderResult<StorePickupCollectionResponse>>
        CollectAsync(
            int orderId ,
            int storeEmployeeId ,
            CollectStorePickupOrderRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( orderId <= 0 )
        {
            return Failure(
                StorePickupCollectionErrorCodes.InvalidRequest ,
                "The order identifier must be greater than zero.");
        }

        if ( storeEmployeeId <= 0 )
        {
            return Failure(
                StorePickupCollectionErrorCodes.InvalidEmployee ,
                "The store employee identifier must be greater than zero.");
        }

        if ( request.PaymentMethod is not (
            PaymentMethod.CashAtStore or
            PaymentMethod.CardAtStore or
            PaymentMethod.InstaPayAtStore ) )
        {
            return Failure(
                StorePickupCollectionErrorCodes.InvalidPaymentMethod ,
                "Store pickup payments must use cash, card, or InstaPay.");
        }

        var paymentReference =
            Normalize(request.PaymentReference);

        if ( paymentReference?.Length >
            MaximumPaymentReferenceLength )
        {
            return Failure(
                StorePickupCollectionErrorCodes.InvalidPaymentReference ,
                $"The payment reference cannot exceed " +
                $"{MaximumPaymentReferenceLength} characters.");
        }

        if ( request.PaymentMethod is
                PaymentMethod.CardAtStore or
                PaymentMethod.InstaPayAtStore &&
            string.IsNullOrWhiteSpace(paymentReference) )
        {
            return Failure(
                StorePickupCollectionErrorCodes.PaymentReferenceRequired ,
                "A payment reference is required for card and InstaPay payments.");
        }

        var notes =
            Normalize(request.Notes);

        if ( notes?.Length > MaximumNotesLength )
        {
            return Failure(
                StorePickupCollectionErrorCodes.InvalidNotes ,
                $"Collection notes cannot exceed " +
                $"{MaximumNotesLength} characters.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable ,
                cancellationToken);

        try
        {
            var employee =
                await _dbContext.Users
                    .SingleOrDefaultAsync(
                        user =>
                            user.Id == storeEmployeeId ,
                        cancellationToken);

            if ( employee is null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return Failure(
                    StorePickupCollectionErrorCodes.EmployeeNotFound ,
                    "The store employee was not found.");
            }

            if ( employee.IsBlocked )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return Failure(
                    StorePickupCollectionErrorCodes.EmployeeBlocked ,
                    "The store employee is blocked.");
            }

            var order =
                await _dbContext.Orders
                    .Include(candidate =>
                        candidate.StatusHistory)
                    .SingleOrDefaultAsync(
                        candidate =>
                            candidate.Id == orderId &&
                            !candidate.IsDeleted ,
                        cancellationToken);

            if ( order is null )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return Failure(
                    StorePickupCollectionErrorCodes.OrderNotFound ,
                    "The order was not found.");
            }

            if ( order.OrderSource != OrderSource.Website ||
                order.DeliveryMethod != DeliveryMethod.StorePickup )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return Failure(
                    StorePickupCollectionErrorCodes.NotStorePickupOrder ,
                    "Only website orders selected for store pickup can be collected.");
            }

            if ( order.OrderStatus == OrderStatus.Delivered &&
                order.PaymentStatus == PaymentStatus.Paid )
            {
                if ( order.PaymentMethod == request.PaymentMethod &&
                    string.Equals(
                        order.PaymentReference ,
                        paymentReference ,
                        StringComparison.OrdinalIgnoreCase) )
                {
                    await transaction.CommitAsync(
                        cancellationToken);

                    return StoreOrderResult<
                        StorePickupCollectionResponse>.Success(
                            CreateResponse(
                                order ,
                                GetCollectingEmployeeId(
                                    order ,
                                    storeEmployeeId)));
                }

                await transaction.RollbackAsync(
                    cancellationToken);

                return Failure(
                    StorePickupCollectionErrorCodes.AlreadyCollected ,
                    "The store pickup order has already been collected.");
            }

            if ( order.PaymentStatus != PaymentStatus.Pending ||
                order.OrderStatus is not (
                    OrderStatus.Pending or
                    OrderStatus.Confirmed or
                    OrderStatus.Preparing ) )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                return Failure(
                    StorePickupCollectionErrorCodes.InvalidOrderStatus ,
                    "The order cannot be collected in its current status.");
            }

            var now =
                _timeProvider.GetUtcNow();

            var actor =
                $"DashboardUser:" +
                storeEmployeeId.ToString(
                    CultureInfo.InvariantCulture);

            var previousStatus =
                order.OrderStatus;

            order.PaymentMethod =
                request.PaymentMethod;

            order.PaymentStatus =
                PaymentStatus.Paid;

            order.PaymentReference =
                paymentReference;

            order.PaidAtUtc =
                now.UtcDateTime;

            order.OrderStatus =
                OrderStatus.Delivered;

            order.LastModifiedOn =
                now;

            order.LastModifiedBy =
                actor;

            order.StatusHistory.Add(
                new OrderStatusHistory
                {
                    PreviousStatus =
                        previousStatus ,

                    NewStatus =
                        OrderStatus.Delivered ,

                    ChangedByUserId =
                        storeEmployeeId ,

                    ActorType =
                        OrderStatusChangeActorType.DashboardUser ,

                    ChangedAtUtc =
                        now.UtcDateTime ,

                    Reason =
                        notes ??
                        "Store pickup order collected and payment confirmed."
                });

            await _dbContext.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return StoreOrderResult<
                StorePickupCollectionResponse>.Success(
                    CreateResponse(
                        order ,
                        storeEmployeeId));
        }
        catch ( DbUpdateConcurrencyException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return Failure(
                StorePickupCollectionErrorCodes.ConcurrencyConflict ,
                "The order changed while collection was being recorded. " +
                "Refresh the order and try again.");
        }
        catch ( DbUpdateException )
        {
            await transaction.RollbackAsync(
                cancellationToken);

            _dbContext.ChangeTracker.Clear();

            return Failure(
                StorePickupCollectionErrorCodes.OperationFailed ,
                "The store pickup collection could not be saved. Please try again.");
        }
    }

    private static StorePickupCollectionResponse CreateResponse(
        Order order ,
        int collectingEmployeeId )
    {
        return new StorePickupCollectionResponse
        {
            OrderId =
                order.Id ,

            OrderNumber =
                order.OrderNumber ,

            OrderStatus =
                order.OrderStatus ,

            PaymentMethod =
                order.PaymentMethod ,

            PaymentStatus =
                order.PaymentStatus ,

            PaymentReference =
                order.PaymentReference ,

            PaidAtUtc =
                order.PaidAtUtc ,

            TotalAmount =
                order.TotalAmount ,

            CollectedByEmployeeId =
                collectingEmployeeId
        };
    }

    private static int GetCollectingEmployeeId(
        Order order ,
        int fallbackEmployeeId )
    {
        return order.StatusHistory
            .Where(history =>
                history.NewStatus ==
                OrderStatus.Delivered)
            .OrderByDescending(history =>
                history.ChangedAtUtc)
            .Select(history =>
                history.ChangedByUserId)
            .FirstOrDefault()
            ?? fallbackEmployeeId;
    }

    private static StoreOrderResult<StorePickupCollectionResponse>
        Failure(
            string code ,
            string message )
    {
        return StoreOrderResult<
            StorePickupCollectionResponse>.Failure(
                code ,
                message);
    }

    private static string? Normalize(
        string? value )
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}