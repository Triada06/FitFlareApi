namespace FitFlare.Application.DTOs.Story;

public class StoryDto
{
    public required string Id { get; set; }
    public required string AuthorId { get; set; }
    public string? AuthorProfilePicture { get; set; }
    public required string MediaType { get; set; }
    public required string MediaSasUrl { get; set; }
    public required DateTime PostedTime { get; set; }
}