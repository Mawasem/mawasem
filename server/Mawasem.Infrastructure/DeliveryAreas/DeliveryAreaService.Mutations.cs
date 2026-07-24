using Mawasem.Application.Features.DeliveryAreas.Contracts.Requests;
using Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;
using Mawasem.Application.Features.DeliveryAreas.Models;
using Mawasem.Domain.Common.ValueObjects;
using Mawasem.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Mawasem.Infrastructure.DeliveryAreas;

public sealed partial class DeliveryAreaService
{
    public async Task<DeliveryAreaResult<DeliveryAreaResponse>>
        CreateAsync(
            int actorUserId ,
            CreateDeliveryAreaRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( actorUserId <= 0 ||
            !Enum.IsDefined(request.Status) )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    "The delivery-area creation request is invalid.");
        }

        if ( !TryNormalizeValues(
                request.NameAr ,
                request.NameEn ,
                request.DeliveryFee ,
                request.IsFreeDelivery ,
                out var nameAr ,
                out var nameEn ,
                out var deliveryFee ,
                out var validationError) )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    validationError);
        }

        if ( await HasDuplicateNameAsync(
                nameAr ,
                nameEn ,
                excludedDeliveryAreaId: null ,
                cancellationToken) )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.DuplicateName ,
                    "A delivery area with the same Arabic or English name already exists.");
        }

        var now = _timeProvider.GetUtcNow();
        var actor = actorUserId.ToString(
            CultureInfo.InvariantCulture);

        var deliveryArea = new DeliveryArea
        {
            Name = new LocalizedText(nameEn , nameAr) ,
            Status = request.Status ,
            DeliveryFee = deliveryFee ,
            IsFreeDelivery = request.IsFreeDelivery ,
            IsActive = request.IsActive ,
            CreatedOn = now ,
            CreatedBy = actor
        };

        _dbContext.DeliveryAreas.Add(deliveryArea);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = await GetResponseByIdAsync(
            deliveryArea.Id ,
            cancellationToken);

        if ( response is null )
        {
            throw new InvalidOperationException(
                "The delivery area was created but could not be reloaded.");
        }

        return DeliveryAreaResult<
            DeliveryAreaResponse>.Success(response);
    }

    public async Task<DeliveryAreaResult<DeliveryAreaResponse>>
        UpdateAsync(
            int actorUserId ,
            int deliveryAreaId ,
            UpdateDeliveryAreaRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( actorUserId <= 0 ||
            deliveryAreaId <= 0 )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    "The delivery-area update request is invalid.");
        }

        if ( !TryNormalizeValues(
                request.NameAr ,
                request.NameEn ,
                request.DeliveryFee ,
                request.IsFreeDelivery ,
                out var nameAr ,
                out var nameEn ,
                out var deliveryFee ,
                out var validationError) )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    validationError);
        }

        var deliveryArea = await _dbContext.DeliveryAreas
            .AsTracking()
            .SingleOrDefaultAsync(
                area =>
                    area.Id == deliveryAreaId &&
                    !area.IsDeleted ,
                cancellationToken);

        if ( deliveryArea is null )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.NotFound ,
                    "The active delivery area was not found.");
        }

        if ( await HasDuplicateNameAsync(
                nameAr ,
                nameEn ,
                deliveryArea.Id ,
                cancellationToken) )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.DuplicateName ,
                    "A delivery area with the same Arabic or English name already exists.");
        }

        deliveryArea.Name.Update(nameEn , nameAr);
        deliveryArea.DeliveryFee = deliveryFee;
        deliveryArea.IsFreeDelivery =
            request.IsFreeDelivery;
        deliveryArea.IsActive = request.IsActive;
        deliveryArea.LastModifiedOn =
            _timeProvider.GetUtcNow();
        deliveryArea.LastModifiedBy =
            actorUserId.ToString(
                CultureInfo.InvariantCulture);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = await GetResponseByIdAsync(
            deliveryArea.Id ,
            cancellationToken);

        if ( response is null )
        {
            throw new InvalidOperationException(
                "The delivery area was updated but could not be reloaded.");
        }

        return DeliveryAreaResult<
            DeliveryAreaResponse>.Success(response);
    }

    public async Task<DeliveryAreaResult<DeliveryAreaResponse>>
        UpdateStatusAsync(
            int actorUserId ,
            int deliveryAreaId ,
            UpdateDeliveryAreaStatusRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( actorUserId <= 0 ||
            deliveryAreaId <= 0 ||
            !Enum.IsDefined(request.Status) )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    "The delivery-area status request is invalid.");
        }

        var deliveryArea = await _dbContext.DeliveryAreas
            .AsTracking()
            .SingleOrDefaultAsync(
                area =>
                    area.Id == deliveryAreaId &&
                    !area.IsDeleted ,
                cancellationToken);

        if ( deliveryArea is null )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.NotFound ,
                    "The active delivery area was not found.");
        }

        deliveryArea.Status = request.Status;
        deliveryArea.LastModifiedOn =
            _timeProvider.GetUtcNow();
        deliveryArea.LastModifiedBy =
            actorUserId.ToString(
                CultureInfo.InvariantCulture);

        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = await GetResponseByIdAsync(
            deliveryArea.Id ,
            cancellationToken);

        if ( response is null )
        {
            throw new InvalidOperationException(
                "The delivery-area status changed but its response could not be reloaded.");
        }

        return DeliveryAreaResult<
            DeliveryAreaResponse>.Success(response);
    }

    public async Task<DeliveryAreaOperationResult> DeleteAsync(
        int actorUserId ,
        int deliveryAreaId ,
        CancellationToken cancellationToken = default )
    {
        if ( actorUserId <= 0 ||
            deliveryAreaId <= 0 )
        {
            return DeliveryAreaOperationResult.Failure(
                DeliveryAreaErrorCodes.InvalidRequest ,
                "The delivery-area deletion request is invalid.");
        }

        var deliveryArea = await _dbContext.DeliveryAreas
            .AsTracking()
            .SingleOrDefaultAsync(
                area =>
                    area.Id == deliveryAreaId &&
                    !area.IsDeleted ,
                cancellationToken);

        if ( deliveryArea is null )
        {
            return DeliveryAreaOperationResult.Failure(
                DeliveryAreaErrorCodes.NotFound ,
                "The active delivery area was not found.");
        }

        var hasActiveAddresses =
            await _dbContext.UserAddresses.AnyAsync(
                address =>
                    address.DeliveryAreaId == deliveryAreaId &&
                    address.IsActive &&
                    !address.IsDeleted ,
                cancellationToken);

        if ( hasActiveAddresses )
        {
            return DeliveryAreaOperationResult.Failure(
                DeliveryAreaErrorCodes.HasActiveAddresses ,
                "The delivery area cannot be deleted while active customer addresses use it. Restrict or deactivate it instead.");
        }

        var now = _timeProvider.GetUtcNow();
        var actor = actorUserId.ToString(
            CultureInfo.InvariantCulture);

        deliveryArea.IsDeleted = true;
        deliveryArea.DeletedOn = now;
        deliveryArea.DeletedBy = actor;
        deliveryArea.LastModifiedOn = now;
        deliveryArea.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return DeliveryAreaOperationResult.Success();
    }

    public async Task<DeliveryAreaOperationResult> RestoreAsync(
        int actorUserId ,
        int deliveryAreaId ,
        CancellationToken cancellationToken = default )
    {
        if ( actorUserId <= 0 ||
            deliveryAreaId <= 0 )
        {
            return DeliveryAreaOperationResult.Failure(
                DeliveryAreaErrorCodes.InvalidRequest ,
                "The delivery-area restoration request is invalid.");
        }

        var deliveryArea = await _dbContext.DeliveryAreas
            .AsTracking()
            .SingleOrDefaultAsync(
                area => area.Id == deliveryAreaId ,
                cancellationToken);

        if ( deliveryArea is null )
        {
            return DeliveryAreaOperationResult.Failure(
                DeliveryAreaErrorCodes.NotFound ,
                "The delivery area was not found.");
        }

        if ( !deliveryArea.IsDeleted )
        {
            return DeliveryAreaOperationResult.Failure(
                DeliveryAreaErrorCodes.InvalidRequest ,
                "The delivery area is not deleted.");
        }

        if ( await HasDuplicateNameAsync(
                deliveryArea.Name.Arabic ,
                deliveryArea.Name.English ,
                deliveryArea.Id ,
                cancellationToken) )
        {
            return DeliveryAreaOperationResult.Failure(
                DeliveryAreaErrorCodes.DuplicateName ,
                "The delivery area cannot be restored because another area uses the same name.");
        }

        var now = _timeProvider.GetUtcNow();
        var actor = actorUserId.ToString(
            CultureInfo.InvariantCulture);

        deliveryArea.IsDeleted = false;
        deliveryArea.DeletedOn = null;
        deliveryArea.DeletedBy = null;
        deliveryArea.LastModifiedOn = now;
        deliveryArea.LastModifiedBy = actor;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return DeliveryAreaOperationResult.Success();
    }
}