using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;

public sealed record DeliveryAreaResponse
{
    public int Id { get; init; }

    public string NameAr { get; init; } = string.Empty;

    public string NameEn { get; init; } = string.Empty;

    public DeliveryAreaStatus Status { get; init; }

    public decimal DeliveryFee { get; init; }

    public decimal EffectiveDeliveryFee { get; init; }

    public bool IsFreeDelivery { get; init; }

    public bool IsActive { get; init; }

    public int ActiveAddressCount { get; init; }

    public bool IsDeleted { get; init; }

    public DateTimeOffset CreatedOn { get; init; }

    public string? CreatedBy { get; init; }

    public DateTimeOffset? LastModifiedOn { get; init; }

    public string? LastModifiedBy { get; init; }

    public DateTimeOffset? DeletedOn { get; init; }

    public string? DeletedBy { get; init; }
}