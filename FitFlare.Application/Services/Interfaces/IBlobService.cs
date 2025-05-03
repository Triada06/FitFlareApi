using Azure.Storage.Blobs.Models;
using FitFlare.Application.DTOs.Blob;
using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.Services.Interfaces;

public interface IBlobService
{
    public Task<BlobDto> GetBlobAsync(string blobName);
    public Task<IEnumerable<string>> GetBlobsAsync();
    public string GetBlobSasUri(string blobName, int expiryMinutes = 10);
    public Task UploadBlobAsync(IFormFile file,string fileName);
    public Task DeleteBlobAsync(string blobName);   
}