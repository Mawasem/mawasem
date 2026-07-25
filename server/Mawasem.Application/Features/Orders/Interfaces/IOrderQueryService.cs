using Mawasem.Application.Features.Orders.Contracts.Requests;
using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Models;

namespace Mawasem.Application.Features.Orders.Interfaces;

public interface IOrderQueryService
{
    Task<OrderQueryResult<CustomerOrderListResponse>>
        GetCustomerListAsync(
            int customerUserId ,
            GetCustomerOrdersRequest request ,
            CancellationToken cancellationToken = default );

    Task<OrderQueryResult<CustomerOrderDetailsResponse>>
        GetCustomerDetailsAsync(
            int customerUserId ,
            int orderId ,
            CancellationToken cancellationToken = default );

    Task<OrderQueryResult<AdminOrderListResponse>>
        GetAdminListAsync(
            GetAdminOrdersRequest request ,
            CancellationToken cancellationToken = default );

    Task<OrderQueryResult<AdminOrderDetailsResponse>>
        GetAdminDetailsAsync(
            int orderId ,
            CancellationToken cancellationToken = default );
}