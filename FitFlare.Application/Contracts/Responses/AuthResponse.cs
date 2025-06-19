namespace FitFlare.Application.Contracts.Responses;

public class AuthResponse
{
    public required string Token { get; set; }
    public DateTime ExpireTime { get; set; }
    public bool IsSucceeded { get; set; }
    public List<string>? Errors { get; set; }
}