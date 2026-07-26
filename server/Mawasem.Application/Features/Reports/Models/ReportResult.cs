namespace Mawasem.Application.Features.Reports.Models;

public sealed record ReportResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static ReportResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(response);

        return new ReportResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static ReportResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorCode);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            errorMessage);

        return new ReportResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}
