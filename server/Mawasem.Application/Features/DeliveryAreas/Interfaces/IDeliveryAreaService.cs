using Mawasem.Application.Features.DeliveryAreas.Contracts.Requests;
using Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;
using Mawasem.Application.Features.DeliveryAreas.Models;

namespace Mawasem.Application.Features.DeliveryAreas.Interfaces;

public interface IDeliveryAreaService
{
    Task<DeliveryAreaResult<PublicDeliveryAreaListResponse>>
        GetPublicListAsync(
            CancellationToken cancellationToken = default );

    Task<DeliveryAreaResult<DeliveryAreaListResponse>>
        GetAdminListAsync(
            GetDeliveryAreasRequest request ,
            CancellationToken cancellationToken = default );

    Task<DeliveryAreaResult<DeliveryAreaResponse>> GetByIdAsync(
        int deliveryAreaId ,
        CancellationToken cancellationToken = default );

    Task<DeliveryAreaResult<DeliveryAreaResponse>> CreateAsync(
        int actorUserId ,
        CreateDeliveryAreaRequest request ,
        CancellationToken cancellationToken = default );

    Task<DeliveryAreaResult<DeliveryAreaResponse>> UpdateAsync(
        int actorUserId ,
        int deliveryAreaId ,
        UpdateDeliveryAreaRequest request ,
        CancellationToken cancellationToken = default );

    Task<DeliveryAreaResult<DeliveryAreaResponse>> UpdateStatusAsync(
        int actorUserId ,
        int deliveryAreaId ,
        UpdateDeliveryAreaStatusRequest request ,
        CancellationToken cancellationToken = default );

    Task<DeliveryAreaOperationResult> DeleteAsync(
        int actorUserId ,
        int deliveryAreaId ,
        CancellationToken cancellationToken = default );

    Task<DeliveryAreaOperationResult> RestoreAsync(
        int actorUserId ,
        int deliveryAreaId ,
        CancellationToken cancellationToken = default );
}