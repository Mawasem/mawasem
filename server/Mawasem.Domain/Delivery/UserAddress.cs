using Mawasem.Domain.Common;
using Mawasem.Domain.Identity;

namespace Mawasem.Domain.Delivery;

public class UserAddress : BaseAuditableEntity
{
    // Customer
    public int UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    // Address information
    public string Label { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string AreaName { get; set; } = string.Empty;

    public string DetailedAddress { get; set; } = string.Empty;

    public string? BuildingNumber { get; set; }

    public string? FloorNumber { get; set; }

    public string? ApartmentNumber { get; set; }

    public string? Landmark { get; set; }

    // Delivery area
    public int DeliveryAreaId { get; set; }

    public DeliveryArea DeliveryArea { get; set; } = null!;

    // Recipient information
    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    // Address settings
    public bool IsDefault { get; set; }

    public bool IsActive { get; set; } = true;
}