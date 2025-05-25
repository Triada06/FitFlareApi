using FitFlare.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.Services.Shared;

public class FileService : IFileService
{
    public bool DeleteFile(string fileName)
    {
        var filePath = Path.Combine("wwwroot/uploads", fileName);

        if (!File.Exists(filePath)) return false;
        File.Delete(filePath);
        return true;
    }
}