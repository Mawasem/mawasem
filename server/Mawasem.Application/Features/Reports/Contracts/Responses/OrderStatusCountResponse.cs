using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record OrderStatusCountResponse
{
    public OrderStatus Status { get; init; }

    public int Count { get; init; }
}
