namespace Mawasem.Application.Features.Orders.Contracts.Responses;

public sealed record AdminOrderCustomerResponse
{
    public int UserId { get; init; }

    public string NameAr { get; init; } =
        string.Empty;

    public string NameEn { get; init; } =
        string.Empty;

    public string Phone { get; init; } =
        string.Empty;
}