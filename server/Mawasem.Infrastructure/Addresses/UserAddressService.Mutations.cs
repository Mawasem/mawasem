using Mawasem.Application.Features.Addresses.Contracts.Requests;
using Mawasem.Application.Features.Addresses.Contracts.Responses;
using Mawasem.Application.Features.Addresses.Models;
using Mawasem.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mawasem.Infrastructure.Addresses;

public sealed partial class UserAddressService
{
    public async Task<UserAddressResult<UserAddressResponse>>
        CreateAsync(
            int userId ,
            CreateUserAddressRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var accessFailure = await ValidateCustomerAsync(
            userId ,
            cancellationToken);

        if ( accessFailure is not null )
        {
            return Failure<UserAddressResponse>(accessFailure);
        }

        if ( !TryNormalize(
                request.Label ,
                request.City ,
                request.AreaName ,
                request.DetailedAddress ,
                request.BuildingNumber ,
                request.FloorNumber ,
                request.ApartmentNumber ,
                request.Landmark ,
                request.RecipientName ,
                request.RecipientPhone ,
                request.DeliveryAreaId ,
                request.CustomDeliveryAreaNameAr ,
                request.CustomDeliveryAreaNameEn ,
                out var input ,
                out var validationError) )
        {
            return UserAddressResult<UserAddressResponse>.Failure(
                UserAddressErrorCodes.InvalidRequest ,
                validationError);
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var deliveryAreaResult =
            await ResolveDeliveryAreaAsync(
                userId ,
                input ,
                allowedCurrentDeliveryAreaId: null ,
                cancellationToken);

        if ( !deliveryAreaResult.Succeeded )
        {
            return UserAddressResult<UserAddressResponse>.Failure(
                deliveryAreaResult.ErrorCode! ,
                deliveryAreaResult.ErrorMessage!);
        }

        var hasActiveAddress =
            await _dbContext.UserAddresses.AnyAsync(
                address =>
                    address.UserId == userId &&
                    address.IsActive &&
                    !address.IsDeleted ,
                cancellationToken);

        var shouldBeDefault =
            request.IsDefault || !hasActiveAddress;

        var now = _timeProvider.GetUtcNow();
        var actor = userId.ToString(
            CultureInfo.InvariantCulture);

        if ( shouldBeDefault )
        {
            await ClearCurrentDefaultAsync(
                userId ,
                now ,
                actor ,
                cancellationToken);
        }

        var address = new UserAddress
        {
            UserId = userId ,
            DeliveryArea = deliveryAreaResult.Response! ,
            Label = input.Label ,
            City = input.City ,
            AreaName = input.AreaName ,
            DetailedAddress = input.DetailedAddress ,
            BuildingNumber = input.BuildingNumber ,
            FloorNumber = input.FloorNumber ,
            ApartmentNumber = input.ApartmentNumber ,
            Landmark = input.Landmark ,
            RecipientName = input.RecipientName ,
            RecipientPhone = input.RecipientPhone ,
            IsDefault = shouldBeDefault ,
            IsActive = true ,
            CreatedOn = now ,
            CreatedBy = actor
        };

        _dbContext.UserAddresses.Add(address);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var response = await GetResponseByIdAsync(
            userId ,
            address.Id ,
            cancellationToken);

        if ( response is null )
        {
            throw new InvalidOperationException(
                "The address was created but could not be reloaded.");
        }

        return UserAddressResult<UserAddressResponse>.Success(
            response);
    }

    public async Task<UserAddressResult<UserAddressResponse>>
        UpdateAsync(
            int userId ,
            int addressId ,
            UpdateUserAddressRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

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

        if ( !TryNormalize(
                request.Label ,
                request.City ,
                request.AreaName ,
                request.DetailedAddress ,
                request.BuildingNumber ,
                request.FloorNumber ,
                request.ApartmentNumber ,
                request.Landmark ,
                request.RecipientName ,
                request.RecipientPhone ,
                request.DeliveryAreaId ,
                request.CustomDeliveryAreaNameAr ,
                request.CustomDeliveryAreaNameEn ,
                out var input ,
                out var validationError) )
        {
            return UserAddressResult<UserAddressResponse>.Failure(
                UserAddressErrorCodes.InvalidRequest ,
                validationError);
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var address = await _dbContext.UserAddresses
            .AsTracking()
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.Id == addressId &&
                    candidate.UserId == userId &&
                    candidate.IsActive &&
                    !candidate.IsDeleted ,
                cancellationToken);

        if ( address is null )
        {
            return UserAddressResult<UserAddressResponse>.Failure(
                UserAddressErrorCodes.AddressNotFound ,
                "The active customer address was not found.");
        }

        var deliveryAreaResult =
            await ResolveDeliveryAreaAsync(
                userId ,
                input ,
                address.DeliveryAreaId ,
                cancellationToken);

        if ( !deliveryAreaResult.Succeeded )
        {
            return UserAddressResult<UserAddressResponse>.Failure(
                deliveryAreaResult.ErrorCode! ,
                deliveryAreaResult.ErrorMessage!);
        }

        address.DeliveryArea =
            deliveryAreaResult.Response!;

        address.Label = input.Label;
        address.City = input.City;
        address.AreaName = input.AreaName;
        address.DetailedAddress = input.DetailedAddress;
        address.BuildingNumber = input.BuildingNumber;
        address.FloorNumber = input.FloorNumber;
        address.ApartmentNumber = input.ApartmentNumber;
        address.Landmark = input.Landmark;
        address.RecipientName = input.RecipientName;
        address.RecipientPhone = input.RecipientPhone;
        address.LastModifiedOn = _timeProvider.GetUtcNow();
        address.LastModifiedBy = userId.ToString(
            CultureInfo.InvariantCulture);

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var response = await GetResponseByIdAsync(
            userId ,
            address.Id ,
            cancellationToken);

        if ( response is null )
        {
            throw new InvalidOperationException(
                "The address was updated but could not be reloaded.");
        }

        return UserAddressResult<UserAddressResponse>.Success(
            response);
    }

    public async Task<UserAddressOperationResult> SetDefaultAsync(
        int userId ,
        int addressId ,
        CancellationToken cancellationToken = default )
    {
        var accessFailure = await ValidateCustomerAsync(
            userId ,
            cancellationToken);

        if ( accessFailure is not null )
        {
            return FailureOperation(accessFailure);
        }

        if ( addressId <= 0 )
        {
            return UserAddressOperationResult.Failure(
                UserAddressErrorCodes.InvalidRequest ,
                "The address identifier is invalid.");
        }

        var addressExists =
            await _dbContext.UserAddresses.AnyAsync(
                address =>
                    address.Id == addressId &&
                    address.UserId == userId &&
                    address.IsActive &&
                    !address.IsDeleted ,
                cancellationToken);

        if ( !addressExists )
        {
            return UserAddressOperationResult.Failure(
                UserAddressErrorCodes.AddressNotFound ,
                "The active customer address was not found.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var now = _timeProvider.GetUtcNow();
        var actor = userId.ToString(
            CultureInfo.InvariantCulture);

        await ClearCurrentDefaultAsync(
            userId ,
            now ,
            actor ,
            cancellationToken);

        var affectedRows = await _dbContext.UserAddresses
            .Where(address =>
                address.Id == addressId &&
                address.UserId == userId &&
                address.IsActive &&
                !address.IsDeleted)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        address => address.IsDefault ,
                        true)
                    .SetProperty(
                        address => address.LastModifiedOn ,
                        now)
                    .SetProperty(
                        address => address.LastModifiedBy ,
                        actor) ,
                cancellationToken);

        if ( affectedRows != 1 )
        {
            return UserAddressOperationResult.Failure(
                UserAddressErrorCodes.AddressNotFound ,
                "The active customer address was not found.");
        }

        await transaction.CommitAsync(cancellationToken);

        return UserAddressOperationResult.Success();
    }

    public async Task<UserAddressOperationResult> DeleteAsync(
        int userId ,
        int addressId ,
        CancellationToken cancellationToken = default )
    {
        var accessFailure = await ValidateCustomerAsync(
            userId ,
            cancellationToken);

        if ( accessFailure is not null )
        {
            return FailureOperation(accessFailure);
        }

        if ( addressId <= 0 )
        {
            return UserAddressOperationResult.Failure(
                UserAddressErrorCodes.InvalidRequest ,
                "The address identifier is invalid.");
        }

        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var address = await _dbContext.UserAddresses
            .AsTracking()
            .SingleOrDefaultAsync(
                candidate =>
                    candidate.Id == addressId &&
                    candidate.UserId == userId &&
                    candidate.IsActive &&
                    !candidate.IsDeleted ,
                cancellationToken);

        if ( address is null )
        {
            return UserAddressOperationResult.Failure(
                UserAddressErrorCodes.AddressNotFound ,
                "The active customer address was not found.");
        }

        var wasDefault = address.IsDefault;
        var now = _timeProvider.GetUtcNow();
        var actor = userId.ToString(
            CultureInfo.InvariantCulture);

        address.IsDefault = false;
        address.IsActive = false;
        address.IsDeleted = true;
        address.DeletedOn = now;
        address.DeletedBy = actor;
        address.LastModifiedOn = now;
        address.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(cancellationToken);

        if ( wasDefault )
        {
            var replacement = await _dbContext.UserAddresses
                .AsTracking()
                .Where(candidate =>
                    candidate.UserId == userId &&
                    candidate.IsActive &&
                    !candidate.IsDeleted)
                .OrderBy(candidate => candidate.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if ( replacement is not null )
            {
                replacement.IsDefault = true;
                replacement.LastModifiedOn = now;
                replacement.LastModifiedBy = actor;

                await _dbContext.SaveChangesAsync(
                    cancellationToken);
            }
        }

        await transaction.CommitAsync(cancellationToken);

        return UserAddressOperationResult.Success();
    }
}