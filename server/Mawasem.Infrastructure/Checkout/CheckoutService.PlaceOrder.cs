using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Contracts.Responses;
using Mawasem.Application.Features.Checkout.Models;
using Mawasem.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Mawasem.Infrastructure.Checkout;

public sealed partial class CheckoutService
{
    public async Task<CheckoutResult<PlaceOrderResponse>>
        PlaceOrderAsync(
            int userId ,
            PlaceOrderRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var idempotencyKey =
            request.IdempotencyKey?.Trim();

        if ( string.IsNullOrWhiteSpace(idempotencyKey) ||
            idempotencyKey.Length >
                MaxIdempotencyKeyLength )
        {
            return CheckoutResult<PlaceOrderResponse>.Failure(
                CheckoutErrorCodes.InvalidIdempotencyKey ,
                $"The idempotency key is required and cannot exceed " +
                $"{MaxIdempotencyKeyLength} characters.");
        }

        await using (
            var transaction =
                await _dbContext.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable ,
                    cancellationToken) )
        {
            try
            {
                var existingOrder =
                    await FindExistingOrderAsync(
                        userId ,
                        idempotencyKey ,
                        cancellationToken);

                if ( existingOrder is not null )
                {
                    await transaction.CommitAsync(
                        cancellationToken);

                    return CheckoutResult<PlaceOrderResponse>.Success(
                        CreatePlaceOrderResponse(
                            existingOrder ,
                            isIdempotentReplay: true));
                }

                var notes =
                    NormalizeOptionalText(
                        request.Notes);

                if ( notes is not null &&
                    notes.Length > MaxNotesLength )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return CheckoutResult<PlaceOrderResponse>.Failure(
                        CheckoutErrorCodes.InvalidNotes ,
                        $"Order notes cannot exceed " +
                        $"{MaxNotesLength} characters.");
                }

                var checkoutResult =
                    await LoadCheckoutContextAsync(
                        userId ,
                        request.UserAddressId ,
                        request.DeliveryMethod ,
                        request.PaymentMethod ,
                        trackEntities: true ,
                        cancellationToken);

                if ( !checkoutResult.Succeeded )
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    return CheckoutResult<PlaceOrderResponse>.Failure(
                        checkoutResult.ErrorCode! ,
                        checkoutResult.ErrorMessage!);
                }

                var checkoutContext =
                    checkoutResult.Response!;

                var now =
                    _timeProvider.GetUtcNow();

                var actor =
                    GetCustomerActor(userId);

                var order =
                    CreateOrder(
                        checkoutContext ,
                        idempotencyKey ,
                        notes ,
                        now ,
                        actor);

                foreach ( var line in checkoutContext.Lines )
                {
                    line.Variant.StockQuantity -=
                        line.CartItem.Quantity;

                    MarkModified(
                        line.Variant ,
                        now ,
                        actor);

                    MarkDeleted(
                        line.CartItem ,
                        now ,
                        actor);
                }

                MarkModified(
                    checkoutContext.Cart ,
                    now ,
                    actor);

                _dbContext.Orders.Add(
                    order);

                await _dbContext.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return CheckoutResult<PlaceOrderResponse>.Success(
                    CreatePlaceOrderResponse(
                        order ,
                        isIdempotentReplay: false));
            }
            catch ( DbUpdateConcurrencyException )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                _dbContext.ChangeTracker.Clear();

                return CheckoutResult<PlaceOrderResponse>.Failure(
                    CheckoutErrorCodes.ConcurrencyConflict ,
                    "Stock changed while the order was being created. " +
                    "Refresh Checkout and try again.");
            }
            catch ( DbUpdateException )
            {
                await transaction.RollbackAsync(
                    cancellationToken);

                _dbContext.ChangeTracker.Clear();
            }
        }

        var duplicateOrder =
            await FindExistingOrderAsync(
                userId ,
                idempotencyKey ,
                cancellationToken);

        if ( duplicateOrder is not null )
        {
            return CheckoutResult<PlaceOrderResponse>.Success(
                CreatePlaceOrderResponse(
                    duplicateOrder ,
                    isIdempotentReplay: true));
        }

        return CheckoutResult<PlaceOrderResponse>.Failure(
            CheckoutErrorCodes.OrderCreationFailed ,
            "The order could not be created. Please try again.");
    }

    private Task<Order?> FindExistingOrderAsync(
        int userId ,
        string idempotencyKey ,
        CancellationToken cancellationToken )
    {
        return _dbContext.Orders
            .AsNoTracking()
            .SingleOrDefaultAsync(
                order =>
                    order.UserId == userId &&
                    order.IdempotencyKey ==
                        idempotencyKey ,
                cancellationToken);
    }
}
