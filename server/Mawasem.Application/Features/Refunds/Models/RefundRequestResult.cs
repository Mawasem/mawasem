namespace Mawasem.Application.Features.Refunds.Models;

public sealed record RefundRequestResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static RefundRequestResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(response);

        return new RefundRequestResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static RefundRequestResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorCode);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorMessage);

        return new RefundRequestResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}