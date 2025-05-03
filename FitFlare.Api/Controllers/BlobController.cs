using System.Reflection.Metadata;
using Azure.Storage.Blobs.Models;
using FitFlare.Application.DTOs.Blob;
using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlobController(IBlobService blobService, IWebHostEnvironment webHostEnvironment)
    : ControllerBase
{
    private readonly IBlobService _blobService = blobService;
    private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetByName(string fileName)
    {
        var blob = await _blobService.GetBlobAsync(fileName);
        return File(blob.Content, blob.ContentType);
    }

    [HttpGet]
    public async Task<IActionResult> GetBlobsAsync()
    {
        var blobs = await _blobService.GetBlobsAsync();
        return Ok(blobs.ToList());
    }

    // [HttpPost("upload")]
    // public async Task<IActionResult> Upload(IFormFile file)
    // {
    //     await _blobService.UploadBlobAsync(file);
    //     return Ok("File uploaded successfully.");
    // }

    [HttpDelete("{fileName}")]
    public async Task<IActionResult> Delete([FromRoute]string fileName)
    {
        await _blobService.DeleteBlobAsync(fileName);
        return Ok("File deleted successfully.");
    }
    [HttpGet("sas")]
    public IActionResult GetBlobSasUrl([FromQuery] string filename)
    {
        var url = _blobService.GetBlobSasUri(filename);
        return Ok(new { url });
    }
}