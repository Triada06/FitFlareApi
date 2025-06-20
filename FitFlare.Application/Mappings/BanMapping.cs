using FitFlare.Application.DTOs.Ban;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Application.Mappings;

public static class BanMapping
{
    public static BanDto MapToBanDto(this Ban ban)
    {
        return new BanDto
        {
            UserId = ban.AppUserId,
            Reason = ban.Reason,
            IsPermanent = ban.IsPermanent,
            ExpiresAt = ban.ExpiresAt,
        };
    }

    public static Ban MapToBan(this BanCreateDto ban)
    {
        return new Ban
        {
            Reason = ban.Reason,
            ExpiresAt = ban.ExpiresAt,
            AppUserId = ban.UserId,
        };
    }
}