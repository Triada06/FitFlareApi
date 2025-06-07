namespace FitFlare.Application.DTOs.AppUser;

public class AppUserContextDto
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public string? ProfilePictureUri { get; set; }
}