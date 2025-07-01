using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;

namespace FitFlare.Infrastructure.Repositories;

public class TagRepository(AppDbContext context) : BaseRepository<Tag>(context), ITagRepository
{
    private readonly AppDbContext _context1 = context;
    public async Task UpdateRangeAsync(IEnumerable<Tag> tags)
    {
        _context1.Tags.UpdateRange(tags);
        await _context1.SaveChangesAsync();
    }
}