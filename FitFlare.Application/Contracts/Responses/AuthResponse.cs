using FitFlare.Application.DTOs.AppUserDTos;

namespace FitFlare.Application.Contracts.Responses;

public class AuthResponse
{
    public required string Token { get; set; }
    public DateTime ExpireTime { get; set; }
}