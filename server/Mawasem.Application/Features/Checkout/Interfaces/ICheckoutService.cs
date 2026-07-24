using Mawasem.Application.Features.Checkout.Contracts.Requests;
using Mawasem.Application.Features.Checkout.Contracts.Responses;
using Mawasem.Application.Features.Checkout.Models;

namespace Mawasem.Application.Features.Checkout.Interfaces;

public interface ICheckoutService
{
    Task<CheckoutResult<CheckoutPreviewResponse>>
        PreviewAsync(
            int userId ,
            CheckoutPreviewRequest request ,
            CancellationToken cancellationToken = default );

    Task<CheckoutResult<PlaceOrderResponse>>
        PlaceOrderAsync(
            int userId ,
            PlaceOrderRequest request ,
            CancellationToken cancellationToken = default );
}