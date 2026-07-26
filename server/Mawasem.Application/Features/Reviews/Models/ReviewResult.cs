namespace Mawasem.Application.Features.Reviews.Models;

public sealed record ReviewResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static ReviewResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(
            response);

        return new ReviewResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static ReviewResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorCode);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorMessage);

        return new ReviewResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}