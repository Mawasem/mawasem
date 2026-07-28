using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.StoreReturns.Contracts.Requests;
using Mawasem.Application.Features.StoreReturns.Interfaces;
using Mawasem.Application.Features.StoreReturns.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/store/orders/{orderId:int}/returns")]
public sealed class AdminStoreReturnsController : ControllerBase
{
    private readonly IStoreReturnService _storeReturnService;

    public AdminStoreReturnsController(
        IStoreReturnService storeReturnService )
    {
        _storeReturnService = storeReturnService;
    }

    [RequirePermission(
        SystemPermissions.Orders.ProcessStoreReturn)]
    [HttpPost]
    public async Task<IActionResult> Create(
        int orderId ,
        [FromBody] CreateStoreReturnRequest request ,
        CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var employeeId) )
        {
            return Unauthorized(
                CreateProblemDetails(
                    StatusCodes.Status401Unauthorized ,
                    "store_returns.invalid_employee" ,
                    "The authenticated store employee is invalid."));
        }

        var result = await _storeReturnService.CreateAsync(
            employeeId ,
            orderId ,
            request ,
            cancellationToken);

        if ( result.Succeeded &&
            result.Response is not null )
        {
            return StatusCode(
                StatusCodes.Status201Created ,
                result.Response);
        }

        var statusCode = result.ErrorCode switch
        {
            StoreReturnErrorCodes.OrderNotFound =>
                StatusCodes.Status404NotFound,

            StoreReturnErrorCodes.OrderItemNotFound =>
                StatusCodes.Status404NotFound,

            StoreReturnErrorCodes.ConcurrencyConflict =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status400BadRequest
        };

        return StatusCode(
            statusCode ,
            CreateProblemDetails(
                statusCode ,
                result.ErrorCode ??
                    StoreReturnErrorCodes.InvalidRequest ,
                result.ErrorMessage ??
                    "The store return could not be completed."));
    }

    private static ProblemDetails CreateProblemDetails(
        int statusCode ,
        string errorCode ,
        string errorMessage )
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode ,
            Title = "Store return request failed." ,
            Detail = errorMessage
        };

        problemDetails.Extensions["code"] =
            errorCode;

        return problemDetails;
    }
}