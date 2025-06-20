namespace FitFlare.Application.DTOs.Admin;

public class DashBoardDto
{
    public IEnumerable<int> BannedUsers { get; set; }
    public IEnumerable<int> UploadedPosts { get; set; }
}