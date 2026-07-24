using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.DeliveryAreas.Contracts.Requests;

public sealed record GetDeliveryAreasRequest
{
    public string? Search { get; init; }

    public DeliveryAreaStatus? Status { get; init; }

    public bool? IsActive { get; init; }

    public bool IncludeDeleted { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}