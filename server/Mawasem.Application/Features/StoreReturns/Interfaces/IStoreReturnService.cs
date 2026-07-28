using Mawasem.Application.Features.StoreReturns.Contracts.Requests;
using Mawasem.Application.Features.StoreReturns.Contracts.Responses;
using Mawasem.Application.Features.StoreReturns.Models;

namespace Mawasem.Application.Features.StoreReturns.Interfaces;

public interface IStoreReturnService
{
    Task<StoreReturnResult<StoreReturnResponse>> CreateAsync(
        int storeEmployeeId ,
        int orderId ,
        CreateStoreReturnRequest request ,
        CancellationToken cancellationToken = default );
}