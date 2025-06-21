namespace FitFlare.Application.DTOs.Admin.Admins;

public class AdminDto
{
    public required string Id { get; set; }
    public required string Role { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public string? ProfilePictureSasUri { get; set; }
}