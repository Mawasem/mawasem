using Mawasem.API.Authorization;
using Mawasem.API.Extensions;
using Mawasem.Application.Features.StoreOrders.Contracts.Requests;
using Mawasem.Application.Features.StoreOrders.Interfaces;
using Mawasem.Application.Features.StoreOrders.Models;
using Mawasem.Domain.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/admin/store/orders")]
public sealed class AdminStoreOrdersController : ControllerBase
{
    private readonly IStoreOrderService _storeOrderService;

    public AdminStoreOrdersController(
        IStoreOrderService storeOrderService )
    {
        _storeOrderService = storeOrderService;
    }

    [RequirePermission(SystemPermissions.Orders.CreateStoreOrder)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateStoreOrderRequest request ,
        CancellationToken cancellationToken )
    {
        if ( !User.TryGetUserId(out var employeeId) )
            return Unauthorized();

        var result = await _storeOrderService.CreateAsync(
            employeeId , request , cancellationToken);

        return CreateResponse(result , created: true);
    }

    [RequirePermission(SystemPermissions.Orders.CreateStoreOrder)]
    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetReceipt(
        int orderId ,
        CancellationToken cancellationToken )
    {
        var result = await _storeOrderService.GetReceiptAsync(
            orderId , cancellationToken);

        return CreateResponse(result , created: false);
    }

    private static IActionResult CreateResponse(
        StoreOrderResult<
            Application.Features.StoreOrders.Contracts.Responses
                .StoreOrderReceiptResponse> result ,
        bool created )
    {
        if ( result.Succeeded && result.Response is not null )
        {
            return created
                ? new CreatedAtActionResult(
                    nameof(GetReceipt) ,
                    "AdminStoreOrders" ,
                    new { orderId = result.Response.OrderId } ,
                    result.Response)
                : new OkObjectResult(result.Response);
        }

        var status = result.ErrorCode switch
        {
            StoreOrderErrorCodes.OrderNotFound => StatusCodes.Status404NotFound,
            StoreOrderErrorCodes.ConcurrencyConflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return new ObjectResult(new ProblemDetails
        {
            Status = status ,
            Title = "Store order request failed." ,
            Detail = result.ErrorMessage
        })
        {
            StatusCode = status
        };
    }
}