namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record CustomerOrderShippingResponse
{
    public int? SourceAddressId { get; init; }

    public int? DeliveryAreaId { get; init; }

    public string? DeliveryAreaNameAr { get; init; }

    public string? DeliveryAreaNameEn { get; init; }

    public string? RecipientName { get; init; }

    public string? RecipientPhone { get; init; }

    public string? City { get; init; }

    public string? AreaName { get; init; }

    public string? DetailedAddress { get; init; }

    public string? BuildingNumber { get; init; }

    public string? FloorNumber { get; init; }

    public string? ApartmentNumber { get; init; }

    public string? Landmark { get; init; }
}