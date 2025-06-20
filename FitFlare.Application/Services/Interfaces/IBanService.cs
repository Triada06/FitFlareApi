using FitFlare.Application.DTOs.Ban;

namespace FitFlare.Application.Services.Interfaces;

public interface IBanService 
{
    public Task BanUser(BanCreateDto banCreateDto);
    public Task RemoveBan(string id);
    public Task RemoveAlBansFromUser(string userId);
    public Task<BanDto> GetById(string id);
    public Task<List<BanDto>> GetAllByUserId(string userId);
    public Task<IEnumerable<int>> GetMonthlyBans();
    public Task<BanDto> Update(string id, BanUpdateDto banUpdateDto);
}