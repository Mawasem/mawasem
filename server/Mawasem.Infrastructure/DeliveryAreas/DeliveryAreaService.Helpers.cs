using Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;
using Mawasem.Domain.Delivery;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.DeliveryAreas;

public sealed partial class DeliveryAreaService
{
    private async Task<DeliveryAreaResponse?>
        GetResponseByIdAsync(
            int deliveryAreaId ,
            CancellationToken cancellationToken )
    {
        var deliveryArea = await _dbContext.DeliveryAreas
            .AsNoTracking()
            .Include(area => area.UserAddresses)
            .SingleOrDefaultAsync(
                area => area.Id == deliveryAreaId ,
                cancellationToken);

        if ( deliveryArea is null )
        {
            return null;
        }

        return CreateResponse(deliveryArea);
    }

    private static DeliveryAreaResponse CreateResponse(
        DeliveryArea deliveryArea )
    {
        return new DeliveryAreaResponse
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
                deliveryArea.IsActive ,
            ActiveAddressCount =
                deliveryArea.UserAddresses.Count(address =>
                    address.IsActive &&
                    !address.IsDeleted) ,
            IsDeleted =
                deliveryArea.IsDeleted ,
            CreatedOn =
                deliveryArea.CreatedOn ,
            CreatedBy =
                deliveryArea.CreatedBy ,
            LastModifiedOn =
                deliveryArea.LastModifiedOn ,
            LastModifiedBy =
                deliveryArea.LastModifiedBy ,
            DeletedOn =
                deliveryArea.DeletedOn ,
            DeletedBy =
                deliveryArea.DeletedBy
        };
    }

    private Task<bool> HasDuplicateNameAsync(
        string nameAr ,
        string nameEn ,
        int? excludedDeliveryAreaId ,
        CancellationToken cancellationToken )
    {
        return _dbContext.DeliveryAreas
            .AsNoTracking()
            .AnyAsync(
                area =>
                    !area.IsDeleted &&
                    ( !excludedDeliveryAreaId.HasValue ||
                     area.Id != excludedDeliveryAreaId.Value ) &&
                    ( area.Name.Arabic == nameAr ||
                     area.Name.English == nameEn ) ,
                cancellationToken);
    }

    private static bool TryNormalizeValues(
        string nameAr ,
        string nameEn ,
        decimal deliveryFee ,
        bool isFreeDelivery ,
        out string normalizedNameAr ,
        out string normalizedNameEn ,
        out decimal normalizedDeliveryFee ,
        out string validationError )
    {
        normalizedNameAr = nameAr?.Trim() ?? string.Empty;
        normalizedNameEn = nameEn?.Trim() ?? string.Empty;
        normalizedDeliveryFee =
            isFreeDelivery
                ? 0m
                : deliveryFee;

        if ( normalizedNameAr.Length == 0 ||
            normalizedNameEn.Length == 0 )
        {
            validationError =
                "Arabic and English delivery-area names are required.";

            return false;
        }

        if ( normalizedNameAr.Length > MaximumNameLength ||
            normalizedNameEn.Length > MaximumNameLength )
        {
            validationError =
                $"Delivery-area names cannot exceed {MaximumNameLength} characters.";

            return false;
        }

        if ( deliveryFee < 0 )
        {
            validationError =
                "Delivery fee cannot be negative.";

            return false;
        }

        validationError = string.Empty;

        return true;
    }
}