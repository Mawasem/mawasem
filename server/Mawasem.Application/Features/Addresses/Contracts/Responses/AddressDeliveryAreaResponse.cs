using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Addresses.Contracts.Responses;

public sealed record AddressDeliveryAreaResponse
{
    public int Id { get; init; }

    public string NameAr { get; init; } = string.Empty;

    public string NameEn { get; init; } = string.Empty;

    public DeliveryAreaStatus Status { get; init; }

    public decimal DeliveryFee { get; init; }

    public decimal EffectiveDeliveryFee { get; init; }

    public bool IsFreeDelivery { get; init; }

    public bool IsActive { get; init; }
}