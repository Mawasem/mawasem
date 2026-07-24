using Mawasem.Application.Features.Addresses.Contracts.Responses;
using Mawasem.Application.Features.Addresses.Models;
using Mawasem.Domain.Common.ValueObjects;
using Mawasem.Domain.Delivery;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mawasem.Infrastructure.Addresses;

public sealed partial class UserAddressService
{
    private async Task<AccessFailure?> ValidateCustomerAsync(
        int userId ,
        CancellationToken cancellationToken )
    {
        if ( userId <= 0 )
        {
            return new AccessFailure(
                UserAddressErrorCodes.InvalidCustomer ,
                "The authenticated customer account was not found.");
        }

        var customer = await _dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new
            {
                user.IsBlocked
            })
            .SingleOrDefaultAsync(cancellationToken);

        if ( customer is null )
        {
            return new AccessFailure(
                UserAddressErrorCodes.InvalidCustomer ,
                "The authenticated customer account was not found.");
        }

        if ( customer.IsBlocked )
        {
            return new AccessFailure(
                UserAddressErrorCodes.AccountBlocked ,
                "The authenticated customer account is blocked.");
        }

        return null;
    }

    private IQueryable<UserAddress> AddressQuery( int userId )
    {
        return _dbContext.UserAddresses
            .AsNoTracking()
            .Include(address => address.DeliveryArea)
            .Where(address =>
                address.UserId == userId &&
                address.IsActive &&
                !address.IsDeleted);
    }

    private async Task<UserAddressResponse?> GetResponseByIdAsync(
        int userId ,
        int addressId ,
        CancellationToken cancellationToken )
    {
        var address = await AddressQuery(userId)
            .SingleOrDefaultAsync(
                candidate => candidate.Id == addressId ,
                cancellationToken);

        return address is null
            ? null
            : CreateResponse(address);
    }

    private static UserAddressResponse CreateResponse(
        UserAddress address )
    {
        var deliveryArea = address.DeliveryArea;

        return new UserAddressResponse
        {
            Id = address.Id ,
            Label = address.Label ,
            City = address.City ,
            AreaName = address.AreaName ,
            DetailedAddress = address.DetailedAddress ,
            BuildingNumber = address.BuildingNumber ,
            FloorNumber = address.FloorNumber ,
            ApartmentNumber = address.ApartmentNumber ,
            Landmark = address.Landmark ,
            RecipientName = address.RecipientName ,
            RecipientPhone = address.RecipientPhone ,
            IsDefault = address.IsDefault ,
            IsActive = address.IsActive ,
            CreatedOn = address.CreatedOn ,
            LastModifiedOn = address.LastModifiedOn ,
            DeliveryArea = new AddressDeliveryAreaResponse
            {
                Id = deliveryArea.Id ,
                NameAr = deliveryArea.Name.Arabic ,
                NameEn = deliveryArea.Name.English ,
                Status = deliveryArea.Status ,
                DeliveryFee = deliveryArea.DeliveryFee ,
                EffectiveDeliveryFee =
                    deliveryArea.IsFreeDelivery
                        ? 0m
                        : deliveryArea.DeliveryFee ,
                IsFreeDelivery =
                    deliveryArea.IsFreeDelivery ,
                IsActive =
                    deliveryArea.IsActive
            }
        };
    }

    private async Task<UserAddressResult<DeliveryArea>>
        ResolveDeliveryAreaAsync(
            int userId ,
            NormalizedAddressInput input ,
            int? allowedCurrentDeliveryAreaId ,
            CancellationToken cancellationToken )
    {
        if ( input.DeliveryAreaId.HasValue )
        {
            var deliveryArea = await _dbContext.DeliveryAreas
                .AsTracking()
                .SingleOrDefaultAsync(
                    area =>
                        area.Id == input.DeliveryAreaId.Value &&
                        !area.IsDeleted ,
                    cancellationToken);

            if ( deliveryArea is null )
            {
                return UserAddressResult<DeliveryArea>.Failure(
                    UserAddressErrorCodes.DeliveryAreaNotFound ,
                    "The selected delivery area was not found.");
            }

            var isCurrentArea =
                allowedCurrentDeliveryAreaId.HasValue &&
                allowedCurrentDeliveryAreaId.Value ==
                deliveryArea.Id;

            if ( !isCurrentArea &&
                ( !deliveryArea.IsActive ||
                 deliveryArea.Status !=
                 DeliveryAreaStatus.Confirmed ) )
            {
                return UserAddressResult<DeliveryArea>.Failure(
                    UserAddressErrorCodes.DeliveryAreaUnavailable ,
                    "The selected delivery area is not available.");
            }

            return UserAddressResult<DeliveryArea>.Success(
                deliveryArea);
        }

        var matchingArea = await _dbContext.DeliveryAreas
            .AsTracking()
            .Where(area =>
                !area.IsDeleted &&
                ( area.Name.Arabic ==
                    input.CustomDeliveryAreaNameAr ||
                 area.Name.English ==
                    input.CustomDeliveryAreaNameEn ))
            .OrderBy(area => area.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if ( matchingArea is not null )
        {
            if ( !matchingArea.IsActive ||
                matchingArea.Status ==
                DeliveryAreaStatus.Restricted )
            {
                return UserAddressResult<DeliveryArea>.Failure(
                    UserAddressErrorCodes.DeliveryAreaUnavailable ,
                    "The requested delivery area is restricted or inactive.");
            }

            return UserAddressResult<DeliveryArea>.Success(
                matchingArea);
        }

        var now = _timeProvider.GetUtcNow();
        var actor = userId.ToString(
            CultureInfo.InvariantCulture);

        var newDeliveryArea = new DeliveryArea
        {
            Name = new LocalizedText(
                input.CustomDeliveryAreaNameEn! ,
                input.CustomDeliveryAreaNameAr!) ,
            Status = DeliveryAreaStatus.Pending ,
            DeliveryFee = 0m ,
            IsFreeDelivery = false ,
            IsActive = true ,
            CreatedOn = now ,
            CreatedBy = actor
        };

        _dbContext.DeliveryAreas.Add(newDeliveryArea);

        return UserAddressResult<DeliveryArea>.Success(
            newDeliveryArea);
    }

    private Task<int> ClearCurrentDefaultAsync(
        int userId ,
        DateTimeOffset now ,
        string actor ,
        CancellationToken cancellationToken )
    {
        return _dbContext.UserAddresses
            .Where(address =>
                address.UserId == userId &&
                address.IsDefault &&
                address.IsActive &&
                !address.IsDeleted)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        address => address.IsDefault ,
                        false)
                    .SetProperty(
                        address => address.LastModifiedOn ,
                        now)
                    .SetProperty(
                        address => address.LastModifiedBy ,
                        actor) ,
                cancellationToken);
    }

    private static bool TryNormalize(
        string label ,
        string city ,
        string areaName ,
        string detailedAddress ,
        string? buildingNumber ,
        string? floorNumber ,
        string? apartmentNumber ,
        string? landmark ,
        string recipientName ,
        string recipientPhone ,
        int? deliveryAreaId ,
        string? customDeliveryAreaNameAr ,
        string? customDeliveryAreaNameEn ,
        out NormalizedAddressInput input ,
        out string validationError )
    {
        input = null!;

        if ( !TryRequired(
                label ,
                MaximumLabelLength ,
                "Address label" ,
                out var normalizedLabel ,
                out validationError) ||
            !TryRequired(
                city ,
                MaximumCityLength ,
                "City" ,
                out var normalizedCity ,
                out validationError) ||
            !TryRequired(
                areaName ,
                MaximumAreaNameLength ,
                "Area name" ,
                out var normalizedAreaName ,
                out validationError) ||
            !TryRequired(
                detailedAddress ,
                MaximumDetailedAddressLength ,
                "Detailed address" ,
                out var normalizedDetailedAddress ,
                out validationError) ||
            !TryRequired(
                recipientName ,
                MaximumRecipientNameLength ,
                "Recipient name" ,
                out var normalizedRecipientName ,
                out validationError) ||
            !TryOptional(
                buildingNumber ,
                MaximumAddressPartLength ,
                "Building number" ,
                out var normalizedBuildingNumber ,
                out validationError) ||
            !TryOptional(
                floorNumber ,
                MaximumAddressPartLength ,
                "Floor number" ,
                out var normalizedFloorNumber ,
                out validationError) ||
            !TryOptional(
                apartmentNumber ,
                MaximumAddressPartLength ,
                "Apartment number" ,
                out var normalizedApartmentNumber ,
                out validationError) ||
            !TryOptional(
                landmark ,
                MaximumLandmarkLength ,
                "Landmark" ,
                out var normalizedLandmark ,
                out validationError) )
        {
            return false;
        }

        if ( !EgyptianPhoneNumberNormalizer.TryNormalize(
                recipientPhone ,
                out var normalizedRecipientPhone) )
        {
            validationError =
                "Enter a valid Egyptian recipient mobile number.";

            return false;
        }

        var hasDeliveryAreaId =
            deliveryAreaId.HasValue;

        var hasCustomArea =
            !string.IsNullOrWhiteSpace(
                customDeliveryAreaNameAr) ||
            !string.IsNullOrWhiteSpace(
                customDeliveryAreaNameEn);

        if ( hasDeliveryAreaId == hasCustomArea )
        {
            validationError =
                "Select one delivery area or provide one custom delivery area.";

            return false;
        }

        string? normalizedCustomNameAr = null;
        string? normalizedCustomNameEn = null;

        if ( hasDeliveryAreaId )
        {
            if ( deliveryAreaId!.Value <= 0 )
            {
                validationError =
                    "The delivery area identifier is invalid.";

                return false;
            }
        }
        else if ( !TryRequired(
                     customDeliveryAreaNameAr! ,
                     MaximumDeliveryAreaNameLength ,
                     "Custom Arabic delivery-area name" ,
                     out normalizedCustomNameAr ,
                     out validationError) ||
                 !TryRequired(
                     customDeliveryAreaNameEn! ,
                     MaximumDeliveryAreaNameLength ,
                     "Custom English delivery-area name" ,
                     out normalizedCustomNameEn ,
                     out validationError) )
        {
            return false;
        }

        input = new NormalizedAddressInput(
            normalizedLabel ,
            normalizedCity ,
            normalizedAreaName ,
            normalizedDetailedAddress ,
            normalizedBuildingNumber ,
            normalizedFloorNumber ,
            normalizedApartmentNumber ,
            normalizedLandmark ,
            normalizedRecipientName ,
            normalizedRecipientPhone ,
            deliveryAreaId ,
            normalizedCustomNameAr ,
            normalizedCustomNameEn);

        validationError = string.Empty;

        return true;
    }

    private static bool TryRequired(
        string value ,
        int maximumLength ,
        string fieldName ,
        out string normalizedValue ,
        out string validationError )
    {
        normalizedValue = value?.Trim() ?? string.Empty;

        if ( normalizedValue.Length == 0 )
        {
            validationError =
                $"{fieldName} is required.";

            return false;
        }

        if ( normalizedValue.Length > maximumLength )
        {
            validationError =
                $"{fieldName} cannot exceed {maximumLength} characters.";

            return false;
        }

        validationError = string.Empty;

        return true;
    }

    private static bool TryOptional(
        string? value ,
        int maximumLength ,
        string fieldName ,
        out string? normalizedValue ,
        out string validationError )
    {
        normalizedValue =
            string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();

        if ( normalizedValue is not null &&
            normalizedValue.Length > maximumLength )
        {
            validationError =
                $"{fieldName} cannot exceed {maximumLength} characters.";

            return false;
        }

        validationError = string.Empty;

        return true;
    }

    private static UserAddressResult<TResponse> Failure<TResponse>(
        AccessFailure failure )
    {
        return UserAddressResult<TResponse>.Failure(
            failure.Code ,
            failure.Message);
    }

    private static UserAddressOperationResult FailureOperation(
        AccessFailure failure )
    {
        return UserAddressOperationResult.Failure(
            failure.Code ,
            failure.Message);
    }

    private sealed record AccessFailure(
        string Code ,
        string Message );

    private sealed record NormalizedAddressInput(
        string Label ,
        string City ,
        string AreaName ,
        string DetailedAddress ,
        string? BuildingNumber ,
        string? FloorNumber ,
        string? ApartmentNumber ,
        string? Landmark ,
        string RecipientName ,
        string RecipientPhone ,
        int? DeliveryAreaId ,
        string? CustomDeliveryAreaNameAr ,
        string? CustomDeliveryAreaNameEn );
}