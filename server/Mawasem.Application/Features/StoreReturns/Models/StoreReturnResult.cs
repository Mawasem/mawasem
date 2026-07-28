namespace Mawasem.Application.Features.StoreReturns.Models;

public sealed record StoreReturnResult<TResponse>
{
    public bool Succeeded { get; init; }
    public TResponse? Response { get; init; }
    public string? ErrorCode { get; init; }
    public string? ErrorMessage { get; init; }

    public static StoreReturnResult<TResponse> Success( TResponse response ) =>
        new() { Succeeded = true , Response = response };

    public static StoreReturnResult<TResponse> Failure(
        string errorCode ,
        string errorMessage ) =>
        new()
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
}