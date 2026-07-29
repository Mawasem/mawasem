using Mawasem.Infrastructure.Storage.Images;
using Microsoft.Extensions.Options;

namespace Mawasem.Tests.Products;

public sealed class AzureBlobProductImageStorageTests
{
    [Fact]
    public void Constructor_ValidOptions_CreatesStorage()
    {
        var storage =
            CreateStorage();

        Assert.NotNull(storage);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-uri")]
    [InlineData("http://storage.example.com")]
    public void Constructor_InvalidServiceUri_Throws(
        string serviceUri )
    {
        var options =
            CreateOptions();

        options.ServiceUri =
            serviceUri;

        Assert.Throws<InvalidOperationException>(
            () => CreateStorage(options));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ab")]
    [InlineData("ProductImages")]
    [InlineData("product_images")]
    [InlineData("-product-images")]
    [InlineData("product-images-")]
    [InlineData("product--images")]
    public void Constructor_InvalidContainerName_Throws(
        string containerName )
    {
        var options =
            CreateOptions();

        options.ContainerName =
            containerName;

        Assert.Throws<InvalidOperationException>(
            () => CreateStorage(options));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-uri")]
    [InlineData("http://cdn.example.com/product-images")]
    public void Constructor_InvalidPublicBaseUrl_Throws(
        string publicBaseUrl )
    {
        var options =
            CreateOptions();

        options.PublicBaseUrl =
            publicBaseUrl;

        Assert.Throws<InvalidOperationException>(
            () => CreateStorage(options));
    }

    [Fact]
    public async Task SaveAsync_ContentTypeDoesNotMatchFile_Throws()
    {
        var storage =
            CreateStorage();

        await using var content =
            new MemoryStream(
                new byte[]
                {
                    0x00 ,
                    0x01 ,
                    0x02 ,
                    0x03
                });

        await Assert.ThrowsAsync<InvalidDataException>(
            () => storage.SaveAsync(
                productId: 1 ,
                content ,
                fileName: "fake.jpg" ,
                contentType: "image/jpeg"));
    }

    [Fact]
    public async Task DeleteAsync_UnsafeStorageKey_Throws()
    {
        var storage =
            CreateStorage();

        await Assert.ThrowsAsync<ArgumentException>(
            () => storage.DeleteAsync(
                "../outside.jpg"));
    }

    private static AzureBlobProductImageStorage
        CreateStorage(
            AzureBlobProductImageStorageOptions? options = null )
    {
        return new AzureBlobProductImageStorage(
            Options.Create(
                options ??
                CreateOptions()));
    }

    private static AzureBlobProductImageStorageOptions
        CreateOptions()
    {
        return new AzureBlobProductImageStorageOptions
        {
            ServiceUri =
                "https://mawasemstorage.blob.core.windows.net" ,
            ContainerName =
                "product-images" ,
            PublicBaseUrl =
                "https://cdn.example.com/product-images"
        };
    }
}