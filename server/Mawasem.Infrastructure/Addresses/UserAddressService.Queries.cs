using Mawasem.Application.Features.Addresses.Contracts.Responses;
using Mawasem.Application.Features.Addresses.Models;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Addresses;

public sealed partial class UserAddressService
{
    public async Task<UserAddressResult<UserAddressListResponse>>
        GetListAsync(
            int userId ,
            CancellationToken cancellationToken = default )
    {
        var accessFailure = await ValidateCustomerAsync(
            userId ,
            cancellationToken);

        if ( accessFailure is not null )
        {
            return Failure<UserAddressListResponse>(accessFailure);
        }

        var addresses = await AddressQuery(userId)
            .OrderByDescending(address => address.IsDefault)
            .ThenBy(address => address.Id)
            .ToArrayAsync(cancellationToken);

        return UserAddressResult<UserAddressListResponse>.Success(
            new UserAddressListResponse
            {
                Items = addresses
                    .Select(CreateResponse)
                    .ToArray()
            });
    }

    public async Task<UserAddressResult<UserAddressResponse>>
        GetByIdAsync(
            int userId ,
            int addressId ,
            CancellationToken cancellationToken = default )
    {
        var accessFailure = await ValidateCustomerAsync(
            userId ,
            cancellationToken);

        if ( accessFailure is not null )
        {
            return Failure<UserAddressResponse>(accessFailure);
        }

        if ( addressId <= 0 )
        {
            return UserAddressResult<UserAddressResponse>.Failure(
                UserAddressErrorCodes.InvalidRequest ,
                "The address identifier is invalid.");
        }

        var response = await GetResponseByIdAsync(
            userId ,
            addressId ,
            cancellationToken);

        if ( response is null )
        {
            return UserAddressResult<UserAddressResponse>.Failure(
                UserAddressErrorCodes.AddressNotFound ,
                "The active customer address was not found.");
        }

        return UserAddressResult<UserAddressResponse>.Success(
            response);
    }
}