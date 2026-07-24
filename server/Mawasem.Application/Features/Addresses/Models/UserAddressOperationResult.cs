namespace Mawasem.Application.Features.Addresses.Models;

public sealed record UserAddressOperationResult
{
    public bool Succeeded { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static UserAddressOperationResult Success()
    {
        return new UserAddressOperationResult
        {
            Succeeded = true
        };
    }

    public static UserAddressOperationResult Failure(
        string errorCode ,
        string errorMessage )
    {
        return new UserAddressOperationResult
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}