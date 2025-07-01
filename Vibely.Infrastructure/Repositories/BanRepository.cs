using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Infrastructure.Repositories;

public class BanRepository(AppDbContext context) : BaseRepository<Ban>(context), IBanRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task RemoveRange(List<Ban> bans)
    {
        _context1.Bans.RemoveRange(bans);
        await _context1.SaveChangesAsync();
    }
}