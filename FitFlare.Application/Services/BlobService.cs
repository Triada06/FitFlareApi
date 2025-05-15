using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using FitFlare.Application.DTOs.Blob;
using FitFlare.Application.Helpers;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FitFlare.Application.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;
    private readonly string _containerName;

    public BlobService(BlobServiceClient blobServiceClient, IOptions<BlobStorageOptions> options,
        IConfiguration configuration)
    {
        _blobServiceClient = blobServiceClient;
        _configuration = configuration;
        _containerName = options.Value.ContainerName; // Now pulls from IOptions
    }

    public async Task<BlobDto> GetBlobAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        var blobDownloadInfo = await blobClient.DownloadAsync();

        return new BlobDto(blobDownloadInfo.Value.Content, blobDownloadInfo.Value.ContentType);
    }

    public async Task<IEnumerable<string>> GetBlobsAsync()
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobs = new List<string>();

        await foreach (var blob in containerClient.GetBlobsAsync())
        {
            blobs.Add(blob.Name);
        }

        return blobs;
    }

    public async Task UploadBlobAsync(IFormFile file, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var blobClient = containerClient.GetBlobClient(fileName);
        var headers = new BlobHttpHeaders
        {
            ContentType = file.ContentType //e.g. image/png, application/pdf, etc.
        };
        await blobClient.UploadAsync(file.OpenReadStream(), headers);
    }
    public string GetBlobSasUri(string blobName, int expiryMinutes = 10)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
}