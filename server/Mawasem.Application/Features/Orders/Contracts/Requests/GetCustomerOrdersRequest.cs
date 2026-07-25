using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Orders.Contracts.Requests;

public sealed record GetCustomerOrdersRequest
{
    public string? Search { get; init; }

    public OrderStatus? Status { get; init; }

    public DateTime? FromDateUtc { get; init; }

    public DateTime? ToDateUtc { get; init; }

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}