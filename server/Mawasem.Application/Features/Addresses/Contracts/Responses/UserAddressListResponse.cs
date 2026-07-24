namespace Mawasem.Application.Features.Addresses.Contracts.Responses;

public sealed record UserAddressListResponse
{
    public IReadOnlyCollection<UserAddressResponse> Items { get; init; } =
        Array.Empty<UserAddressResponse>();
}