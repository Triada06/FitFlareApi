using Microsoft.AspNetCore.Http;

namespace FitFlare.Application.DTOs.Posts;

public class PostAnalyseDto
{
    public required string UserId { get; set; }
    public required string LocalFileName { get; set; }
    public required IFormFile Media { get; set; }
    public string Status { get; set; } = "Drafted";
}