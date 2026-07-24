using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.DeliveryAreas.Contracts.Requests;

public sealed record UpdateDeliveryAreaStatusRequest
{
    public DeliveryAreaStatus Status { get; init; }
}