namespace Mawasem.Application.Features.Orders.Models;

public sealed record OrderQueryResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static OrderQueryResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(response);

        return new OrderQueryResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static OrderQueryResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorCode);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorMessage);

        return new OrderQueryResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}