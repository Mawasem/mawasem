namespace Mawasem.Application.Features.Addresses.Models;

public static class UserAddressErrorCodes
{
    public const string InvalidCustomer =
        "addresses.invalid_customer";

    public const string AccountBlocked =
        "addresses.account_blocked";

    public const string InvalidRequest =
        "addresses.invalid_request";

    public const string AddressNotFound =
        "addresses.not_found";

    public const string DeliveryAreaNotFound =
        "addresses.delivery_area_not_found";

    public const string DeliveryAreaUnavailable =
        "addresses.delivery_area_unavailable";
}