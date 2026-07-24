namespace Mawasem.Application.Features.Addresses.Contracts.Requests;

public sealed record UpdateUserAddressRequest
{
    public string Label { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string AreaName { get; init; } = string.Empty;

    public string DetailedAddress { get; init; } = string.Empty;

    public string? BuildingNumber { get; init; }

    public string? FloorNumber { get; init; }

    public string? ApartmentNumber { get; init; }

    public string? Landmark { get; init; }

    public string RecipientName { get; init; } = string.Empty;

    public string RecipientPhone { get; init; } = string.Empty;

    public int? DeliveryAreaId { get; init; }

    public string? CustomDeliveryAreaNameAr { get; init; }

    public string? CustomDeliveryAreaNameEn { get; init; }
}