using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Mawasem.Application.Features.Products.Interfaces;
using Mawasem.Application.Features.Products.Models;
using Microsoft.Extensions.Options;

namespace Mawasem.Infrastructure.Storage.Images;

public sealed class AzureBlobProductImageStorage
    : IProductImageStorage
{
    private const long MaximumImageLength =
        10 * 1024 * 1024;

    private readonly BlobContainerClient
        _containerClient;

    private readonly string _publicBaseUrl;

    public AzureBlobProductImageStorage(
        IOptions<AzureBlobProductImageStorageOptions> options )
    {
        ArgumentNullException.ThrowIfNull(options);

        var storageOptions =
            options.Value;

        var serviceUri =
            ValidateServiceUri(
                storageOptions.ServiceUri);

        var containerName =
            ValidateContainerName(
                storageOptions.ContainerName);

        _publicBaseUrl =
            ValidatePublicBaseUrl(
                storageOptions.PublicBaseUrl);

        var credential =
            new DefaultAzureCredential(
                new DefaultAzureCredentialOptions
                {
                    ExcludeInteractiveBrowserCredential = true
                });

        var serviceClient =
            new BlobServiceClient(
                serviceUri ,
                credential);

        _containerClient =
            serviceClient.GetBlobContainerClient(
                containerName);
    }

    public async Task<StoredProductImage>
        SaveAsync(
            int productId ,
            Stream content ,
            string fileName ,
            string contentType ,
            CancellationToken cancellationToken = default )
    {
        if ( productId <= 0 )
        {
            throw new ArgumentOutOfRangeException(
                nameof(productId));
        }

        ArgumentNullException.ThrowIfNull(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentType);

        if ( !content.CanRead )
        {
            throw new ArgumentException(
                "The image content stream must be readable." ,
                nameof(content));
        }

        var normalizedContentType =
            NormalizeContentType(
                contentType);

        var extension =
            GetFileExtension(
                normalizedContentType);

        var signature =
            new byte[12];

        var signatureLength =
            await ReadSignatureAsync(
                content ,
                signature ,
                cancellationToken);

        if ( !HasValidSignature(
                signature.AsSpan(
                    0 ,
                    signatureLength) ,
                normalizedContentType) )
        {
            throw new InvalidDataException(
                "The uploaded file content does not match its image content type.");
        }

        await using var uploadContent =
            new MemoryStream();

        await uploadContent.WriteAsync(
            signature.AsMemory(
                0 ,
                signatureLength) ,
            cancellationToken);

        var copyBuffer =
            new byte[81920];

        long totalLength =
            signatureLength;

        while ( true )
        {
            var bytesRead =
                await content.ReadAsync(
                    copyBuffer.AsMemory(
                        0 ,
                        copyBuffer.Length) ,
                    cancellationToken);

            if ( bytesRead == 0 )
            {
                break;
            }

            totalLength +=
                bytesRead;

            if ( totalLength >
                MaximumImageLength )
            {
                throw new InvalidDataException(
                    "The selected image cannot exceed 10 MB.");
            }

            await uploadContent.WriteAsync(
                copyBuffer.AsMemory(
                    0 ,
                    bytesRead) ,
                cancellationToken);
        }

        if ( totalLength <= 0 )
        {
            throw new InvalidDataException(
                "The selected image is empty.");
        }

        uploadContent.Position =
            0;

        var storageKey =
            $"{productId}/" +
            $"{Guid.NewGuid():N}.{extension}";

        var blobClient =
            _containerClient.GetBlobClient(
                storageKey);

        var uploadOptions =
            new BlobUploadOptions
            {
                HttpHeaders =
                    new BlobHttpHeaders
                    {
                        ContentType =
                            normalizedContentType ,
                        CacheControl =
                            "public, max-age=31536000, immutable"
                    } ,
                Conditions =
                    new BlobRequestConditions
                    {
                        IfNoneMatch =
                            ETag.All
                    }
            };

        await blobClient.UploadAsync(
            uploadContent ,
            uploadOptions ,
            cancellationToken);

        var imageUrl =
            $"{_publicBaseUrl}/{storageKey}";

        return new StoredProductImage(
            storageKey ,
            imageUrl);
    }

    public async Task DeleteAsync(
        string storageKey ,
        CancellationToken cancellationToken = default )
    {
        var normalizedStorageKey =
            NormalizeStorageKey(
                storageKey);

        var blobClient =
            _containerClient.GetBlobClient(
                normalizedStorageKey);

        await blobClient.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots ,
            conditions: null ,
            cancellationToken:
                cancellationToken);
    }

    private static async Task<int>
        ReadSignatureAsync(
            Stream content ,
            byte[] signature ,
            CancellationToken cancellationToken )
    {
        var totalBytesRead =
            0;

        while ( totalBytesRead <
            signature.Length )
        {
            var bytesRead =
                await content.ReadAsync(
                    signature.AsMemory(
                        totalBytesRead ,
                        signature.Length -
                            totalBytesRead) ,
                    cancellationToken);

            if ( bytesRead == 0 )
            {
                break;
            }

            totalBytesRead +=
                bytesRead;
        }

        return totalBytesRead;
    }

    private static bool HasValidSignature(
        ReadOnlySpan<byte> signature ,
        string contentType )
    {
        return contentType switch
        {
            "image/jpeg" =>
                signature.Length >= 3 &&
                signature[0] == 0xFF &&
                signature[1] == 0xD8 &&
                signature[2] == 0xFF,

            "image/png" =>
                signature.Length >= 8 &&
                signature[0] == 0x89 &&
                signature[1] == 0x50 &&
                signature[2] == 0x4E &&
                signature[3] == 0x47 &&
                signature[4] == 0x0D &&
                signature[5] == 0x0A &&
                signature[6] == 0x1A &&
                signature[7] == 0x0A,

            "image/webp" =>
                signature.Length >= 12 &&
                signature[0] == 0x52 &&
                signature[1] == 0x49 &&
                signature[2] == 0x46 &&
                signature[3] == 0x46 &&
                signature[8] == 0x57 &&
                signature[9] == 0x45 &&
                signature[10] == 0x42 &&
                signature[11] == 0x50,

            _ =>
                false
        };
    }

    private static string GetFileExtension(
        string contentType )
    {
        return contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png" => "png",
            "image/webp" => "webp",
            _ => throw new InvalidDataException(
                "Only JPEG, PNG, and WebP images are supported.")
        };
    }

    private static string NormalizeContentType(
        string contentType )
    {
        return contentType
            .Split(
                ';' ,
                StringSplitOptions.RemoveEmptyEntries)[0]
            .Trim()
            .ToLowerInvariant();
    }

    private static string NormalizeStorageKey(
        string storageKey )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            storageKey);

        var normalizedStorageKey =
            storageKey
                .Trim()
                .Replace(
                    '\\' ,
                    '/');

        if ( normalizedStorageKey.StartsWith('/') ||
            normalizedStorageKey.EndsWith('/') ||
            normalizedStorageKey.Contains("//" , StringComparison.Ordinal) )
        {
            throw new ArgumentException(
                "The image storage key is invalid." ,
                nameof(storageKey));
        }

        var segments =
            normalizedStorageKey.Split('/');

        if ( segments.Any(segment =>
                string.IsNullOrWhiteSpace(segment) ||
                segment == "." ||
                segment == "..") )
        {
            throw new ArgumentException(
                "The image storage key is invalid." ,
                nameof(storageKey));
        }

        return normalizedStorageKey;
    }

    private static Uri ValidateServiceUri(
        string serviceUri )
    {
        if ( !Uri.TryCreate(
                serviceUri?.Trim() ,
                UriKind.Absolute ,
                out var parsedUri) ||
            parsedUri.Scheme !=
                Uri.UriSchemeHttps )
        {
            throw new InvalidOperationException(
                "ProductImageStorage:AzureBlob:ServiceUri must be a valid HTTPS URI.");
        }

        return parsedUri;
    }

    private static string ValidateContainerName(
        string containerName )
    {
        var normalizedContainerName =
            containerName?.Trim();

        if ( string.IsNullOrWhiteSpace(
                normalizedContainerName) ||
            normalizedContainerName.Length < 3 ||
            normalizedContainerName.Length > 63 ||
            normalizedContainerName[0] == '-' ||
            normalizedContainerName[^1] == '-' ||
            normalizedContainerName.Contains(
                "--" ,
                StringComparison.Ordinal) ||
            normalizedContainerName.Any(character =>
                !( character is >= 'a' and <= 'z' ) &&
                !( character is >= '0' and <= '9' ) &&
                character != '-') )
        {
            throw new InvalidOperationException(
                "ProductImageStorage:AzureBlob:ContainerName is invalid.");
        }

        return normalizedContainerName;
    }

    private static string ValidatePublicBaseUrl(
        string publicBaseUrl )
    {
        var normalizedPublicBaseUrl =
            publicBaseUrl?
                .Trim()
                .TrimEnd('/');

        if ( !Uri.TryCreate(
                normalizedPublicBaseUrl ,
                UriKind.Absolute ,
                out var parsedUri) ||
            parsedUri.Scheme !=
                Uri.UriSchemeHttps )
        {
            throw new InvalidOperationException(
                "ProductImageStorage:AzureBlob:PublicBaseUrl must be a valid HTTPS URI.");
        }

        return normalizedPublicBaseUrl;
    }
}