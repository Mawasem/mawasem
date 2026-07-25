using Mawasem.Application.Features.Orders.Contracts.Requests;
using Mawasem.Application.Features.Orders.Contracts.Responses;
using Mawasem.Application.Features.Orders.Interfaces;
using Mawasem.Application.Features.Orders.Models;
using Mawasem.Domain.Enums;
using Mawasem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Mawasem.Infrastructure.Orders;

public sealed class OrderQueryService
    : IOrderQueryService
{
    private const int MaximumPageSize = 100;

    private const int MaximumSearchLength = 100;

    private readonly MawasemDbContext _dbContext;

    public OrderQueryService(
        MawasemDbContext dbContext )
    {
        _dbContext = dbContext;
    }

    public async Task<
        OrderQueryResult<CustomerOrderListResponse>>
        GetCustomerListAsync(
            int customerUserId ,
            GetCustomerOrdersRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        if ( customerUserId <= 0 )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( request.PageNumber <= 0 )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    "Page number must be greater than zero.");
        }

        if ( request.PageSize <= 0 ||
            request.PageSize > MaximumPageSize )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    $"Page size must be between 1 and {MaximumPageSize}.");
        }

        var skipCount =
            (long)( request.PageNumber - 1 ) *
            request.PageSize;

        if ( skipCount > int.MaxValue )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    "The requested page is outside the supported range.");
        }

        if ( request.Status.HasValue &&
            !Enum.IsDefined(request.Status.Value) )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    "The order status is invalid.");
        }

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
            request.ToDateUtc.Value )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    "The start date cannot be later than the end date.");
        }

        var search =
            request.Search?.Trim();

        if ( search?.Length > MaximumSearchLength )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    $"Search text cannot exceed {MaximumSearchLength} characters.");
        }

        var customer =
            await _dbContext.Users
                .AsNoTracking()
                .Where(user =>
                    user.Id == customerUserId)
                .Select(user =>
                    new
                    {
                        user.IsBlocked
                    })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( customer is null )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( customer.IsBlocked )
        {
            return OrderQueryResult<
                CustomerOrderListResponse>.Failure(
                    OrderQueryErrorCodes.CustomerBlocked ,
                    "The customer account is blocked.");
        }

        var query =
            _dbContext.Orders
                .AsNoTracking()
                .Where(order =>
                    !order.IsDeleted &&
                    order.UserId == customerUserId);

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            query =
                query.Where(order =>
                    order.OrderNumber.Contains(search));
        }

        if ( request.Status.HasValue )
        {
            query =
                query.Where(order =>
                    order.OrderStatus ==
                    request.Status.Value);
        }

        if ( request.FromDateUtc.HasValue )
        {
            query =
                query.Where(order =>
                    order.OrderDate >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            query =
                query.Where(order =>
                    order.OrderDate <=
                    request.ToDateUtc.Value);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(order =>
                    order.OrderDate)
                .ThenByDescending(order =>
                    order.Id)
                .Skip((int)skipCount)
                .Take(request.PageSize)
                .Select(order =>
                    new CustomerOrderListItemResponse
                    {
                        Id =
                            order.Id ,

                        OrderNumber =
                            order.OrderNumber ,

                        OrderDate =
                            order.OrderDate ,

                        OrderStatus =
                            order.OrderStatus ,

                        PaymentMethod =
                            order.PaymentMethod ,

                        PaymentStatus =
                            order.PaymentStatus ,

                        DeliveryMethod =
                            order.DeliveryMethod ,

                        SubTotal =
                            order.SubTotal ,

                        Discount =
                            order.Discount ,

                        DeliveryFee =
                            order.DeliveryFee ,

                        TotalAmount =
                            order.TotalAmount ,

                        DistinctItemCount =
                            order.OrderItems.Count(item =>
                                !item.IsDeleted) ,

                        TotalQuantity =
                            order.OrderItems
                                .Where(item =>
                                    !item.IsDeleted)
                                .Select(item =>
                                    (int?)item.Quantity)
                                .Sum()
                            ?? 0 ,

                        CanCancel =
                            order.OrderStatus ==
                            OrderStatus.Pending
                    })
                .ToArrayAsync(
                    cancellationToken);

        var totalPages =
            totalCount == 0
                ? 0
                : (int)Math.Ceiling(
                    totalCount /
                    (double)request.PageSize);

        var response =
            new CustomerOrderListResponse
            {
                Items = items ,
                PageNumber = request.PageNumber ,
                PageSize = request.PageSize ,
                TotalCount = totalCount ,
                TotalPages = totalPages
            };

        return OrderQueryResult<
            CustomerOrderListResponse>.Success(
                response);
    }

    public async Task<
        OrderQueryResult<CustomerOrderDetailsResponse>>
        GetCustomerDetailsAsync(
            int customerUserId ,
            int orderId ,
            CancellationToken cancellationToken = default )
    {
        if ( customerUserId <= 0 )
        {
            return OrderQueryResult<
                CustomerOrderDetailsResponse>.Failure(
                    OrderQueryErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( orderId <= 0 )
        {
            return OrderQueryResult<
                CustomerOrderDetailsResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    "The order identifier must be greater than zero.");
        }

        var customer =
            await _dbContext.Users
                .AsNoTracking()
                .Where(user =>
                    user.Id == customerUserId)
                .Select(user =>
                    new
                    {
                        user.IsBlocked
                    })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( customer is null )
        {
            return OrderQueryResult<
                CustomerOrderDetailsResponse>.Failure(
                    OrderQueryErrorCodes.CustomerNotFound ,
                    "The customer was not found.");
        }

        if ( customer.IsBlocked )
        {
            return OrderQueryResult<
                CustomerOrderDetailsResponse>.Failure(
                    OrderQueryErrorCodes.CustomerBlocked ,
                    "The customer account is blocked.");
        }

        var response =
            await _dbContext.Orders
                .AsNoTracking()
                .Where(order =>
                    order.Id == orderId &&
                    order.UserId == customerUserId &&
                    !order.IsDeleted)
                .Select(order =>
                    new CustomerOrderDetailsResponse
                    {
                        Id =
                            order.Id ,

                        OrderNumber =
                            order.OrderNumber ,

                        OrderDate =
                            order.OrderDate ,

                        OrderStatus =
                            order.OrderStatus ,

                        PaymentMethod =
                            order.PaymentMethod ,

                        PaymentStatus =
                            order.PaymentStatus ,

                        DeliveryMethod =
                            order.DeliveryMethod ,

                        OrderSource =
                            order.OrderSource ,

                        SubTotal =
                            order.SubTotal ,

                        Discount =
                            order.Discount ,

                        DeliveryFee =
                            order.DeliveryFee ,

                        TotalAmount =
                            order.TotalAmount ,

                        CouponCode =
                            order.CouponCode ,

                        Notes =
                            order.Notes ,

                        CancellationReason =
                            order.CancellationReason ,

                        CancelledAtUtc =
                            order.CancelledAtUtc ,

                        RejectionReason =
                            order.RejectionReason ,

                        RejectedAtUtc =
                            order.RejectedAtUtc ,

                        DistinctItemCount =
                            order.OrderItems.Count(item =>
                                !item.IsDeleted) ,

                        TotalQuantity =
                            order.OrderItems
                                .Where(item =>
                                    !item.IsDeleted)
                                .Select(item =>
                                    (int?)item.Quantity)
                                .Sum()
                            ?? 0 ,

                        CanCancel =
                            order.OrderStatus ==
                            OrderStatus.Pending ,

                        Shipping =
                            new CustomerOrderShippingResponse
                            {
                                SourceAddressId =
                                    order.UserAddressId ,

                                DeliveryAreaId =
                                    order.ShippingDeliveryAreaId ,

                                DeliveryAreaNameAr =
                                    order.ShippingDeliveryAreaNameAr ,

                                DeliveryAreaNameEn =
                                    order.ShippingDeliveryAreaNameEn ,

                                RecipientName =
                                    order.ShippingRecipientName ,

                                RecipientPhone =
                                    order.ShippingRecipientPhone ,

                                City =
                                    order.ShippingCity ,

                                AreaName =
                                    order.ShippingAreaName ,

                                DetailedAddress =
                                    order.ShippingDetailedAddress ,

                                BuildingNumber =
                                    order.ShippingBuildingNumber ,

                                FloorNumber =
                                    order.ShippingFloorNumber ,

                                ApartmentNumber =
                                    order.ShippingApartmentNumber ,

                                Landmark =
                                    order.ShippingLandmark
                            } ,

                        Items =
                            order.OrderItems
                                .Where(item =>
                                    !item.IsDeleted)
                                .OrderBy(item =>
                                    item.Id)
                                .Select(item =>
                                    new CustomerOrderItemResponse
                                    {
                                        Id =
                                            item.Id ,

                                        ProductId =
                                            item.ProductId ,

                                        ProductVariantId =
                                            item.ProductVariantId ,

                                        ProductNameAr =
                                            item.ProductNameAr ,

                                        ProductNameEn =
                                            item.ProductNameEn ,

                                        Sku =
                                            item.SKU ,

                                        VariantSummaryAr =
                                            item.VariantSummaryAr ,

                                        VariantSummaryEn =
                                            item.VariantSummaryEn ,

                                        UnitPrice =
                                            item.UnitPrice ,

                                        DiscountAmount =
                                            item.DiscountAmount ,

                                        Quantity =
                                            item.Quantity ,

                                        LineTotal =
                                            item.TotalPrice ,

                                        RefundedQuantity =
                                            item.RefundedQuantity
                                    })
                                .ToArray()
                    })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( response is null )
        {
            return OrderQueryResult<
                CustomerOrderDetailsResponse>.Failure(
                    OrderQueryErrorCodes.OrderNotFound ,
                    "The order was not found.");
        }

        return OrderQueryResult<
            CustomerOrderDetailsResponse>.Success(
                response);
    }

    public async Task<
        OrderQueryResult<AdminOrderListResponse>>
        GetAdminListAsync(
            GetAdminOrdersRequest request ,
            CancellationToken cancellationToken = default )
    {
        ArgumentNullException.ThrowIfNull(request);

        var validationError =
            ValidateAdminListRequest(request);

        if ( validationError is not null )
        {
            return OrderQueryResult<
                AdminOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    validationError);
        }

        var skipCount =
            (long)( request.PageNumber - 1 ) *
            request.PageSize;

        if ( skipCount > int.MaxValue )
        {
            return OrderQueryResult<
                AdminOrderListResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    "The requested page is outside the supported range.");
        }

        var search =
            request.Search?.Trim();

        var query =
            _dbContext.Orders
                .AsNoTracking()
                .Where(order =>
                    !order.IsDeleted);

        if ( !string.IsNullOrWhiteSpace(search) )
        {
            query =
                query.Where(order =>
                    order.OrderNumber.Contains(search) ||
                    order.CustomerNameAr.Contains(search) ||
                    order.CustomerNameEn.Contains(search) ||
                    order.CustomerPhone.Contains(search));
        }

        if ( request.CustomerUserId.HasValue )
        {
            query =
                query.Where(order =>
                    order.UserId ==
                    request.CustomerUserId.Value);
        }

        if ( request.Status.HasValue )
        {
            query =
                query.Where(order =>
                    order.OrderStatus ==
                    request.Status.Value);
        }

        if ( request.PaymentMethod.HasValue )
        {
            query =
                query.Where(order =>
                    order.PaymentMethod ==
                    request.PaymentMethod.Value);
        }

        if ( request.PaymentStatus.HasValue )
        {
            query =
                query.Where(order =>
                    order.PaymentStatus ==
                    request.PaymentStatus.Value);
        }

        if ( request.DeliveryMethod.HasValue )
        {
            query =
                query.Where(order =>
                    order.DeliveryMethod ==
                    request.DeliveryMethod.Value);
        }

        if ( request.OrderSource.HasValue )
        {
            query =
                query.Where(order =>
                    order.OrderSource ==
                    request.OrderSource.Value);
        }

        if ( request.DeliveryAreaId.HasValue )
        {
            query =
                query.Where(order =>
                    order.ShippingDeliveryAreaId ==
                    request.DeliveryAreaId.Value);
        }

        if ( request.FromDateUtc.HasValue )
        {
            query =
                query.Where(order =>
                    order.OrderDate >=
                    request.FromDateUtc.Value);
        }

        if ( request.ToDateUtc.HasValue )
        {
            query =
                query.Where(order =>
                    order.OrderDate <=
                    request.ToDateUtc.Value);
        }

        var totalCount =
            await query.CountAsync(
                cancellationToken);

        var items =
            await query
                .OrderByDescending(order =>
                    order.OrderDate)
                .ThenByDescending(order =>
                    order.Id)
                .Skip((int)skipCount)
                .Take(request.PageSize)
                .Select(order =>
                    new AdminOrderListItemResponse
                    {
                        Id =
                            order.Id ,

                        OrderNumber =
                            order.OrderNumber ,

                        OrderDate =
                            order.OrderDate ,

                        CustomerUserId =
                            order.UserId ,

                        CustomerNameAr =
                            order.CustomerNameAr ,

                        CustomerNameEn =
                            order.CustomerNameEn ,

                        CustomerPhone =
                            order.CustomerPhone ,

                        OrderStatus =
                            order.OrderStatus ,

                        PaymentMethod =
                            order.PaymentMethod ,

                        PaymentStatus =
                            order.PaymentStatus ,

                        DeliveryMethod =
                            order.DeliveryMethod ,

                        OrderSource =
                            order.OrderSource ,

                        ShippingDeliveryAreaId =
                            order.ShippingDeliveryAreaId ,

                        ShippingDeliveryAreaNameAr =
                            order.ShippingDeliveryAreaNameAr ,

                        ShippingDeliveryAreaNameEn =
                            order.ShippingDeliveryAreaNameEn ,

                        SubTotal =
                            order.SubTotal ,

                        Discount =
                            order.Discount ,

                        DeliveryFee =
                            order.DeliveryFee ,

                        TotalAmount =
                            order.TotalAmount ,

                        DistinctItemCount =
                            order.OrderItems.Count(item =>
                                !item.IsDeleted) ,

                        TotalQuantity =
                            order.OrderItems
                                .Where(item =>
                                    !item.IsDeleted)
                                .Select(item =>
                                    (int?)item.Quantity)
                                .Sum()
                            ?? 0 ,

                        CanConfirm =
                            order.OrderStatus ==
                            OrderStatus.Pending ,

                        CanReject =
                            order.OrderStatus ==
                            OrderStatus.Pending ,

                        CanCancel =
                            order.OrderStatus ==
                                OrderStatus.Pending ||
                            order.OrderStatus ==
                                OrderStatus.Confirmed
                    })
                .ToArrayAsync(
                    cancellationToken);

        var totalPages =
            totalCount == 0
                ? 0
                : (int)Math.Ceiling(
                    totalCount /
                    (double)request.PageSize);

        var response =
            new AdminOrderListResponse
            {
                Items = items ,
                PageNumber = request.PageNumber ,
                PageSize = request.PageSize ,
                TotalCount = totalCount ,
                TotalPages = totalPages
            };

        return OrderQueryResult<
            AdminOrderListResponse>.Success(
                response);
    }

    public async Task<
    OrderQueryResult<AdminOrderDetailsResponse>>
    GetAdminDetailsAsync(
        int orderId ,
        CancellationToken cancellationToken = default )
    {
        if ( orderId <= 0 )
        {
            return OrderQueryResult<
                AdminOrderDetailsResponse>.Failure(
                    OrderQueryErrorCodes.InvalidRequest ,
                    "The order identifier must be greater than zero.");
        }

        var response =
            await _dbContext.Orders
                .AsNoTracking()
                .Where(order =>
                    order.Id == orderId &&
                    !order.IsDeleted)
                .Select(order =>
                    new AdminOrderDetailsResponse
                    {
                        Id =
                            order.Id ,

                        OrderNumber =
                            order.OrderNumber ,

                        OrderDate =
                            order.OrderDate ,

                        OrderStatus =
                            order.OrderStatus ,

                        PaymentMethod =
                            order.PaymentMethod ,

                        PaymentStatus =
                            order.PaymentStatus ,

                        DeliveryMethod =
                            order.DeliveryMethod ,

                        OrderSource =
                            order.OrderSource ,

                        SubTotal =
                            order.SubTotal ,

                        Discount =
                            order.Discount ,

                        DeliveryFee =
                            order.DeliveryFee ,

                        TotalAmount =
                            order.TotalAmount ,

                        CouponCode =
                            order.CouponCode ,

                        Notes =
                            order.Notes ,

                        IdempotencyKey =
                            order.IdempotencyKey ,

                        CancellationReason =
                            order.CancellationReason ,

                        CancelledAtUtc =
                            order.CancelledAtUtc ,

                        RejectionReason =
                            order.RejectionReason ,

                        RejectedAtUtc =
                            order.RejectedAtUtc ,

                        StockRestoredAtUtc =
                            order.StockRestoredAtUtc ,

                        DistinctItemCount =
                            order.OrderItems.Count(item =>
                                !item.IsDeleted) ,

                        TotalQuantity =
                            order.OrderItems
                                .Where(item =>
                                    !item.IsDeleted)
                                .Select(item =>
                                    (int?)item.Quantity)
                                .Sum()
                            ?? 0 ,

                        CanConfirm =
                            order.OrderStatus ==
                            OrderStatus.Pending ,

                        CanReject =
                            order.OrderStatus ==
                            OrderStatus.Pending ,

                        CanCancel =
                            order.OrderStatus ==
                                OrderStatus.Pending ||
                            order.OrderStatus ==
                                OrderStatus.Confirmed ,

                        Customer =
                            new AdminOrderCustomerResponse
                            {
                                UserId =
                                    order.UserId ,

                                NameAr =
                                    order.CustomerNameAr ,

                                NameEn =
                                    order.CustomerNameEn ,

                                Phone =
                                    order.CustomerPhone
                            } ,

                        Shipping =
                            new CustomerOrderShippingResponse
                            {
                                SourceAddressId =
                                    order.UserAddressId ,

                                DeliveryAreaId =
                                    order.ShippingDeliveryAreaId ,

                                DeliveryAreaNameAr =
                                    order.ShippingDeliveryAreaNameAr ,

                                DeliveryAreaNameEn =
                                    order.ShippingDeliveryAreaNameEn ,

                                RecipientName =
                                    order.ShippingRecipientName ,

                                RecipientPhone =
                                    order.ShippingRecipientPhone ,

                                City =
                                    order.ShippingCity ,

                                AreaName =
                                    order.ShippingAreaName ,

                                DetailedAddress =
                                    order.ShippingDetailedAddress ,

                                BuildingNumber =
                                    order.ShippingBuildingNumber ,

                                FloorNumber =
                                    order.ShippingFloorNumber ,

                                ApartmentNumber =
                                    order.ShippingApartmentNumber ,

                                Landmark =
                                    order.ShippingLandmark
                            } ,

                        Items =
                            order.OrderItems
                                .Where(item =>
                                    !item.IsDeleted)
                                .OrderBy(item =>
                                    item.Id)
                                .Select(item =>
                                    new CustomerOrderItemResponse
                                    {
                                        Id =
                                            item.Id ,

                                        ProductId =
                                            item.ProductId ,

                                        ProductVariantId =
                                            item.ProductVariantId ,

                                        ProductNameAr =
                                            item.ProductNameAr ,

                                        ProductNameEn =
                                            item.ProductNameEn ,

                                        Sku =
                                            item.SKU ,

                                        VariantSummaryAr =
                                            item.VariantSummaryAr ,

                                        VariantSummaryEn =
                                            item.VariantSummaryEn ,

                                        UnitPrice =
                                            item.UnitPrice ,

                                        DiscountAmount =
                                            item.DiscountAmount ,

                                        Quantity =
                                            item.Quantity ,

                                        LineTotal =
                                            item.TotalPrice ,

                                        RefundedQuantity =
                                            item.RefundedQuantity
                                    })
                                .ToArray()
                    })
                .SingleOrDefaultAsync(
                    cancellationToken);

        if ( response is null )
        {
            return OrderQueryResult<
                AdminOrderDetailsResponse>.Failure(
                    OrderQueryErrorCodes.OrderNotFound ,
                    "The order was not found.");
        }

        return OrderQueryResult<
            AdminOrderDetailsResponse>.Success(
                response);
    }

    private static string? ValidateAdminListRequest(
        GetAdminOrdersRequest request )
    {
        if ( request.PageNumber <= 0 )
        {
            return "Page number must be greater than zero.";
        }

        if ( request.PageSize <= 0 ||
            request.PageSize > MaximumPageSize )
        {
            return
                $"Page size must be between 1 and {MaximumPageSize}.";
        }

        var search =
            request.Search?.Trim();

        if ( search?.Length > MaximumSearchLength )
        {
            return
                $"Search text cannot exceed {MaximumSearchLength} characters.";
        }

        if ( request.CustomerUserId.HasValue &&
            request.CustomerUserId.Value <= 0 )
        {
            return
                "The customer identifier must be greater than zero.";
        }

        if ( request.DeliveryAreaId.HasValue &&
            request.DeliveryAreaId.Value <= 0 )
        {
            return
                "The delivery-area identifier must be greater than zero.";
        }

        if ( request.Status.HasValue &&
            !Enum.IsDefined(request.Status.Value) )
        {
            return "The order status is invalid.";
        }

        if ( request.PaymentMethod.HasValue &&
            !Enum.IsDefined(request.PaymentMethod.Value) )
        {
            return "The payment method is invalid.";
        }

        if ( request.PaymentStatus.HasValue &&
            !Enum.IsDefined(request.PaymentStatus.Value) )
        {
            return "The payment status is invalid.";
        }

        if ( request.DeliveryMethod.HasValue &&
            !Enum.IsDefined(request.DeliveryMethod.Value) )
        {
            return "The delivery method is invalid.";
        }

        if ( request.OrderSource.HasValue &&
            !Enum.IsDefined(request.OrderSource.Value) )
        {
            return "The order source is invalid.";
        }

        if ( request.FromDateUtc.HasValue &&
            request.ToDateUtc.HasValue &&
            request.FromDateUtc.Value >
            request.ToDateUtc.Value )
        {
            return
                "The start date cannot be later than the end date.";
        }

        return null;
    }
}