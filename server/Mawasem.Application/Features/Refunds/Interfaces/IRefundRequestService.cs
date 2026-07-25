using Mawasem.Application.Features.Refunds.Contracts.Requests;
using Mawasem.Application.Features.Refunds.Contracts.Responses;
using Mawasem.Application.Features.Refunds.Models;

namespace Mawasem.Application.Features.Refunds.Interfaces;

public interface IRefundRequestService
{
    Task<RefundRequestResult<RefundRequestDetailsResponse>>
        CreateAsync(
            int orderId ,
            int customerUserId ,
            CreateRefundRequestRequest request ,
            CancellationToken cancellationToken = default );

    Task<RefundRequestResult<CustomerRefundRequestListResponse>>
        GetCustomerListAsync(
            int customerUserId ,
            GetCustomerRefundRequestsRequest request ,
            CancellationToken cancellationToken = default );

    Task<RefundRequestResult<RefundRequestDetailsResponse>>
        GetCustomerDetailsAsync(
            int customerUserId ,
            int refundRequestId ,
            CancellationToken cancellationToken = default );

    Task<RefundRequestResult<AdminRefundRequestListResponse>>
        GetAdminListAsync(
            GetAdminRefundRequestsRequest request ,
            CancellationToken cancellationToken = default );

    Task<RefundRequestResult<AdminRefundRequestDetailsResponse>>
        GetAdminDetailsAsync(
            int refundRequestId ,
            CancellationToken cancellationToken = default );

    Task<RefundRequestResult<AdminRefundRequestDetailsResponse>>
        ApproveAsync(
            int refundRequestId ,
            int dashboardUserId ,
            ApproveRefundRequestRequest request ,
            CancellationToken cancellationToken = default );

    Task<RefundRequestResult<AdminRefundRequestDetailsResponse>>
        RejectAsync(
            int refundRequestId ,
            int dashboardUserId ,
            RejectRefundRequestRequest request ,
            CancellationToken cancellationToken = default );

    Task<RefundRequestResult<AdminRefundRequestDetailsResponse>>
        CompleteAsync(
            int refundRequestId ,
            int dashboardUserId ,
            CompleteRefundRequestRequest request ,
            CancellationToken cancellationToken = default );
}