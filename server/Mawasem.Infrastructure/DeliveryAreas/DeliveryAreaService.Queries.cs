using Mawasem.Application.Features.DeliveryAreas.Contracts.Requests;
using Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;
using Mawasem.Application.Features.DeliveryAreas.Models;
using Mawasem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.DeliveryAreas;

public sealed partial class DeliveryAreaService
{
    public async Task<
        DeliveryAreaResult<PublicDeliveryAreaListResponse>>
        GetPublicListAsync(
            CancellationToken cancellationToken = default )
    {
        var items = await _dbContext.DeliveryAreas
            .AsNoTracking()
            .Where(area =>
                !area.IsDeleted &&
                area.IsActive &&
                area.Status == DeliveryAreaStatus.Confirmed)
            .OrderBy(area => area.Name.English)
            .ThenBy(area => area.Id)
            .Select(area => new PublicDeliveryAreaResponse
            {
                Id = area.Id ,
                NameAr = area.Name.Arabic ,
                NameEn = area.Name.English ,
                DeliveryFee =
                    area.IsFreeDelivery
                        ? 0m
                        : area.DeliveryFee ,
                IsFreeDelivery = area.IsFreeDelivery
            })
            .ToArrayAsync(cancellationToken);

        return DeliveryAreaResult<
            PublicDeliveryAreaListResponse>.Success(
                new PublicDeliveryAreaListResponse
                {
                    Items = items
                });
    }

    public async Task<DeliveryAreaResult<DeliveryAreaListResponse>>
        GetAdminListAsync(
            GetDeliveryAreasRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( request.PageNumber <= 0 ||
            request.PageSize <= 0 ||
            request.PageSize > MaximumPageSize )
        {
            return DeliveryAreaResult<
                DeliveryAreaListResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    $"Page number must be positive and page size must be between 1 and {MaximumPageSize}.");
        }

        if ( request.Status.HasValue &&
            !Enum.IsDefined(request.Status.Value) )
        {
            return DeliveryAreaResult<
                DeliveryAreaListResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    "The delivery-area status is invalid.");
        }

        var search = request.Search?.Trim();

        if ( search?.Length > MaximumSearchLength )
        {
            return DeliveryAreaResult<
                DeliveryAreaListResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    $"Search cannot exceed {MaximumSearchLength} characters.");
        }

        var query = _dbContext.DeliveryAreas
            .AsNoTracking()
            .AsQueryable();

        if ( !request.IncludeDeleted )
        {
            query = query.Where(area => !area.IsDeleted);
        }

        if ( request.Status.HasValue )
        {
            query = query.Where(area =>
                area.Status == request.Status.Value);
        }

        if ( request.IsActive.HasValue )
        {
            query = query.Where(area =>
                area.IsActive == request.IsActive.Value);
        }

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            query = query.Where(area =>
                area.Name.Arabic.Contains(search) ||
                area.Name.English.Contains(search));
        }

        var totalCount = await query.CountAsync(
            cancellationToken);

        var items = await query
            .OrderBy(area => area.Status)
            .ThenBy(area => area.Name.English)
            .ThenBy(area => area.Id)
            .Skip(( request.PageNumber - 1 ) * request.PageSize)
            .Take(request.PageSize)
            .Select(area => new DeliveryAreaResponse
            {
                Id = area.Id ,
                NameAr = area.Name.Arabic ,
                NameEn = area.Name.English ,
                Status = area.Status ,
                DeliveryFee = area.DeliveryFee ,
                EffectiveDeliveryFee =
                    area.IsFreeDelivery
                        ? 0m
                        : area.DeliveryFee ,
                IsFreeDelivery = area.IsFreeDelivery ,
                IsActive = area.IsActive ,
                ActiveAddressCount =
                    area.UserAddresses.Count(address =>
                        address.IsActive &&
                        !address.IsDeleted) ,
                IsDeleted = area.IsDeleted ,
                CreatedOn = area.CreatedOn ,
                CreatedBy = area.CreatedBy ,
                LastModifiedOn = area.LastModifiedOn ,
                LastModifiedBy = area.LastModifiedBy ,
                DeletedOn = area.DeletedOn ,
                DeletedBy = area.DeletedBy
            })
            .ToArrayAsync(cancellationToken);

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(
                totalCount / (double)request.PageSize);

        return DeliveryAreaResult<
            DeliveryAreaListResponse>.Success(
                new DeliveryAreaListResponse
                {
                    Items = items ,
                    PageNumber = request.PageNumber ,
                    PageSize = request.PageSize ,
                    TotalCount = totalCount ,
                    TotalPages = totalPages
                });
    }

    public async Task<DeliveryAreaResult<DeliveryAreaResponse>>
        GetByIdAsync(
            int deliveryAreaId ,
            CancellationToken cancellationToken = default )
    {
        if ( deliveryAreaId <= 0 )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.InvalidRequest ,
                    "The delivery-area identifier is invalid.");
        }

        var response = await GetResponseByIdAsync(
            deliveryAreaId ,
            cancellationToken);

        if ( response is null )
        {
            return DeliveryAreaResult<
                DeliveryAreaResponse>.Failure(
                    DeliveryAreaErrorCodes.NotFound ,
                    "The delivery area was not found.");
        }

        return DeliveryAreaResult<
            DeliveryAreaResponse>.Success(response);
    }
}