namespace Mawasem.Application.Features.DeliveryAreas.Models;

public sealed record DeliveryAreaOperationResult
{
    public bool Succeeded { get; init; }

    public string? ErrorCode { get; init; }

    public string? ErrorMessage { get; init; }

    public static DeliveryAreaOperationResult Success()
    {
        return new DeliveryAreaOperationResult
        {
            Succeeded = true
        };
    }

    public static DeliveryAreaOperationResult Failure(
        string errorCode ,
        string errorMessage )
    {
        return new DeliveryAreaOperationResult
        {
            Succeeded = false ,
            ErrorCode = errorCode ,
            ErrorMessage = errorMessage
        };
    }
}