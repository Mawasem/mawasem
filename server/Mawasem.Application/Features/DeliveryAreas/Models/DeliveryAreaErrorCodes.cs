namespace Mawasem.Application.Features.DeliveryAreas.Models;

public static class DeliveryAreaErrorCodes
{
    public const string InvalidRequest =
        "delivery_areas.invalid_request";

    public const string NotFound =
        "delivery_areas.not_found";

    public const string DuplicateName =
        "delivery_areas.duplicate_name";

    public const string HasActiveAddresses =
        "delivery_areas.has_active_addresses";
}