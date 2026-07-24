namespace Mawasem.Application.Features.DeliveryAreas.Contracts.Responses;

public sealed record PublicDeliveryAreaListResponse
{
    public IReadOnlyCollection<PublicDeliveryAreaResponse> Items
    {
        get;
        init;
    } = Array.Empty<PublicDeliveryAreaResponse>();
}