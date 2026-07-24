using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.DeliveryAreas.Contracts.Requests;

public sealed record CreateDeliveryAreaRequest
{
    public string NameAr { get; init; } = string.Empty;

    public string NameEn { get; init; } = string.Empty;

    public decimal DeliveryFee { get; init; }

    public bool IsFreeDelivery { get; init; }

    public bool IsActive { get; init; } = true;

    public DeliveryAreaStatus Status { get; init; } =
        DeliveryAreaStatus.Confirmed;
}