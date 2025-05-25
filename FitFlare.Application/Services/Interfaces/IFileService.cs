using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.Services.Interfaces;

public interface IFileService
{
    public bool DeleteFile(string fileName);
}