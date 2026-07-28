using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Contracts.Responses;
using Mawasem.Application.Features.StoreOrders.Models;

namespace Mawasem.Application.Features.StoreOrders.Interfaces;

public interface IStoreOrderService
{
    Task<StoreOrderResult<StoreOrderReceiptResponse>>
        CreateAsync(
            int storeEmployeeId ,
            CreateStoreOrderRequest request ,
            CancellationToken cancellationToken = default );

    Task<StoreOrderResult<StoreOrderReceiptResponse>>
        GetReceiptAsync(
            int orderId ,
            CancellationToken cancellationToken = default );
}