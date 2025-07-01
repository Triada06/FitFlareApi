using FitFlare.Application.DTOs.Ban;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Mappings;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Application.Services;

public class BanService(IBanRepository banRepository, IAppUserRepository appUserRepository) : IBanService
{
    public async Task BanUser(BanCreateDto banCreateDto)
    {
        var user = await appUserRepository.GetByIdAsync(banCreateDto.UserId);
        if (user == null)
            throw new UserNotFoundException();
        var ban = banCreateDto.MapToBan();
        await banRepository.CreateAsync(ban);
    }   

    public async Task RemoveBan(string id)
    {
        var ban = await banRepository.GetByIdAsync(id);
        if (ban is null) throw new NotFoundException("Ban not found");
        await banRepository.DeleteAsync(ban);
    }

    public async Task RemoveAlBansFromUser(string userId)
    {
        var user = await appUserRepository.GetByIdAsync(userId, i => i.Include(m => m.Bans));
        if (user == null)
            throw new UserNotFoundException();
        var bans = user.Bans.ToList();
        await banRepository.RemoveRange(bans);
    }

    public async Task<BanDto> GetById(string id)
    {
        var ban = await banRepository.GetByIdAsync(id);
        if (ban is null) throw new NotFoundException("Ban not found");
        return ban.MapToBanDto();
    }

    public async Task<List<BanDto>> GetAllByUserId(string userId)
    {
        var user = await appUserRepository.GetByIdAsync(userId, i => i.Include(m => m.Bans), tracking: false);
        if (user == null)
            throw new UserNotFoundException();
        return user.Bans.Select(m => m.MapToBanDto()).ToList();
    }

    public async Task<IEnumerable<int>> GetMonthlyBans()
    {
        var result = await banRepository
            .GetAllAsync(tracking: false);
        result = result.ToList();
        var sortedData = result.GroupBy(b => b.CreatedAt.Month)
            .Select(g => new { Month = g.Key, Count = g.Count() });
        var data = new int[12];

        foreach (var item in sortedData)
        {
            data[item.Month - 1] = item.Count;
        }

        return data;
    }

    public async Task<BanDto> Update(string id, BanUpdateDto banUpdateDto)
    {
        var ban = await banRepository.GetByIdAsync(id);
        if (ban is null) throw new NotFoundException("Ban not found");
        ban.Reason = banUpdateDto.Reason;
        ban.ExpiresAt = banUpdateDto.ExpiresAt;
        await banRepository.UpdateAsync(ban);
        return ban.MapToBanDto();
    }
}