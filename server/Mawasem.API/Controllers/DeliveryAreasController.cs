using Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;
using Mawasem.Application.Features.DeliveryAreas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mawasem.API.Controllers;

[ApiController]
[Route("api/delivery-areas")]
public sealed class DeliveryAreasController : ControllerBase
{
    private readonly IDeliveryAreaService _deliveryAreaService;

    public DeliveryAreasController(
        IDeliveryAreaService deliveryAreaService )
    {
        _deliveryAreaService = deliveryAreaService;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(
        typeof(PublicDeliveryAreaListResponse) ,
        StatusCodes.Status200OK)]
    public async Task<
        ActionResult<PublicDeliveryAreaListResponse>>
        GetListAsync(
            CancellationToken cancellationToken )
    {
        var result =
            await _deliveryAreaService.GetPublicListAsync(
                cancellationToken);

        if ( !result.Succeeded )
        {
            var problemDetails = new ProblemDetails
            {
                Status =
                    StatusCodes.Status400BadRequest ,
                Title =
                    "Delivery-area request failed." ,
                Detail =
                    result.ErrorMessage ??
                    "The delivery areas could not be returned."
            };

            problemDetails.Extensions["code"] =
                result.ErrorCode ??
                "delivery_areas.invalid_request";

            return BadRequest(problemDetails);
        }

        return Ok(result.Response);
    }
}