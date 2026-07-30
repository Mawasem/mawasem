using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Contracts.Responses;
using Mawasem.Application.Features.Checkout.Models;

namespace Mawasem.Infrastructure.Checkout;

public sealed partial class CheckoutService
{
    public async Task<CheckoutResult<CheckoutPreviewResponse>>
        PreviewAsync(
            int userId ,
            CheckoutPreviewRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var checkoutResult =
            await LoadCheckoutContextAsync(
                userId ,
                request.UserAddressId ,
                request.PaymentMethod ,
                trackEntities: false ,
                cancellationToken);

        if ( !checkoutResult.Succeeded )
        {
            return CheckoutResult<CheckoutPreviewResponse>.Failure(
                checkoutResult.ErrorCode! ,
                checkoutResult.ErrorMessage!);
        }

        var response =
            CreatePreviewResponse(
                checkoutResult.Response! ,
                request.PaymentMethod);

        return CheckoutResult<CheckoutPreviewResponse>.Success(
            response);
    }
}