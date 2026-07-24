using Mawasem.Application.Features.Addresses.Contracts.Requests;
using Mawasem.Application.Features.Addresses.Contracts.Responses;
using Mawasem.Application.Features.Addresses.Models;

namespace Mawasem.Application.Features.Addresses.Interfaces;

public interface IUserAddressService
{
    Task<UserAddressResult<UserAddressListResponse>> GetListAsync(
        int userId ,
        CancellationToken cancellationToken = default );

    Task<UserAddressResult<UserAddressResponse>> GetByIdAsync(
        int userId ,
        int addressId ,
        CancellationToken cancellationToken = default );

    Task<UserAddressResult<UserAddressResponse>> CreateAsync(
        int userId ,
        CreateUserAddressRequest request ,
        CancellationToken cancellationToken = default );

    Task<UserAddressResult<UserAddressResponse>> UpdateAsync(
        int userId ,
        int addressId ,
        UpdateUserAddressRequest request ,
        CancellationToken cancellationToken = default );

    Task<UserAddressOperationResult> SetDefaultAsync(
        int userId ,
        int addressId ,
        CancellationToken cancellationToken = default );

    Task<UserAddressOperationResult> DeleteAsync(
        int userId ,
        int addressId ,
        CancellationToken cancellationToken = default );
}