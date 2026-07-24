namespace Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;

public sealed record PublicDeliveryAreaResponse
{
    public int Id { get; init; }

    public string NameAr { get; init; } = string.Empty;

    public string NameEn { get; init; } = string.Empty;

    public decimal DeliveryFee { get; init; }

    public bool IsFreeDelivery { get; init; }
}