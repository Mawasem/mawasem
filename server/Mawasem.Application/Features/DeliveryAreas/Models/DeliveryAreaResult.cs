namespace Mawasem.Application.Features.DeliveryAreas.Models;

public sealed record DeliveryAreaResult<TResponse>
{
    public bool Succeeded { get; init; }

    public TResponse? Response { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static DeliveryAreaResult<TResponse> Success(
        TResponse response )
    {
        ArgumentNullException.ThrowIfNull(response);

        return new DeliveryAreaResult<TResponse>
        {
            Succeeded = true ,
            Response = response
        };
    }

    public static DeliveryAreaResult<TResponse> Failure(
        string errorCode ,
        string errorMessage )
    {
        return new DeliveryAreaResult<TResponse>
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}