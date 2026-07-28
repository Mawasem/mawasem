using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Contracts.Responses;
using Mawasem.Application.Features.StoreOrders.Models;

namespace Mawasem.Application.Features.StoreOrders.Interfaces;

public interface IStorePickupCollectionService
{
    Task<StoreOrderResult<StorePickupCollectionResponse>>
        CollectAsync(
            int orderId ,
            int storeEmployeeId ,
            CollectStorePickupOrderRequest request ,
            CancellationToken cancellationToken = default );
}