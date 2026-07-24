using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Models;

namespace Mawasem.Application.Features.Orders.Interfaces;

public interface IOrderWorkflowService
{
    Task<OrderWorkflowResult<OrderWorkflowResponse>>
        ConfirmAsync(
            int orderId ,
            int dashboardUserId ,
            CancellationToken cancellationToken = default );

    Task<OrderWorkflowResult<OrderWorkflowResponse>>
        RejectAsync(
            int orderId ,
            int dashboardUserId ,
            string reason ,
            CancellationToken cancellationToken = default );

    Task<OrderWorkflowResult<OrderWorkflowResponse>>
        CancelByDashboardAsync(
            int orderId ,
            int dashboardUserId ,
            string reason ,
            CancellationToken cancellationToken = default );

    Task<OrderWorkflowResult<OrderWorkflowResponse>>
        CancelByCustomerAsync(
            int orderId ,
            int customerUserId ,
            string reason ,
            CancellationToken cancellationToken = default );
}