namespace Mawasem.Application.Features.Orders.Contracts.Requests;

public sealed record CancelOrderRequest
{
    public string Reason { get; init; } =
        string.Empty;
}