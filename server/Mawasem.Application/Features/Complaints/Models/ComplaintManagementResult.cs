namespace Mawasem.Application.Features.Complaints.Models;

public sealed record ComplaintManagementResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static ComplaintManagementResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(response);

        return new ComplaintManagementResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static ComplaintManagementResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        return new ComplaintManagementResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}
