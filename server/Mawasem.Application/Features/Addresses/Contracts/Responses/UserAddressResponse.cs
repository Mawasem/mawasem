namespace Mawasem.Application.Features.Addresses.Contracts.Responses;

public sealed record UserAddressResponse
{
    public int Id { get; init; }

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

    public bool IsDefault { get; init; }

    public bool IsActive { get; init; }

    public AddressDeliveryAreaResponse DeliveryArea { get; init; } =
        new();

    public DateTimeOffset CreatedOn { get; init; }

    public DateTimeOffset? LastModifiedOn { get; init; }
}