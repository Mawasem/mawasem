using Mawasem.Domain.Enums;

namespace Mawasem.Application.Features.Reports.Contracts.Responses;

public sealed record EmployeeOrderActionCountResponse
{
    public OrderStatus ActionStatus { get; init; }

    public int Count { get; init; }
}
