namespace Mawasem.Infrastructure.Storage.Images;

public sealed class AzureBlobProductImageStorageOptions
{
    public const string SectionName =
        "ProductImageStorage:AzureBlob";

    public string ServiceUri { get; set; } =
        string.Empty;

    public string ContainerName { get; set; } =
        "product-images";

    public string PublicBaseUrl { get; set; } =
        string.Empty;
}