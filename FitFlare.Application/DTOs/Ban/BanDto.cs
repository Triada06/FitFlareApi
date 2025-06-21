namespace FitFlare.Application.DTOs.Ban;

public class BanDto
{
    public required string Id { get; set; }
    public required string UserId { get; set; }
    public required string Reason { get; set; }
    public required bool IsPermanent{ get; set; }
    public DateTime BannedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}