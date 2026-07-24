namespace Mawasem.Application.Features.Addresses.Models;

public sealed record UserAddressResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static UserAddressResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(response);

        return new UserAddressResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static UserAddressResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        return new UserAddressResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}