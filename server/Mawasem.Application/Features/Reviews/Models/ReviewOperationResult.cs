namespace Mawasem.Application.Features.Reviews.Models;

public sealed record ReviewOperationResult
{
    public bool Succeeded { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static ReviewOperationResult Success()
    {
        return new ReviewOperationResult
        {
            Succeeded = true
        };
    }

    public static ReviewOperationResult Failure(
        string errorCode ,
        string errorMessage )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorCode);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorMessage);

        return new ReviewOperationResult
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}