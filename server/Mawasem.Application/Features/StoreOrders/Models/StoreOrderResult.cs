namespace Mawasem.Application.Features.StoreOrders.Models;

public sealed record StoreOrderResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static StoreOrderResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(response);

        return new StoreOrderResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static StoreOrderResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        return new StoreOrderResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}