using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class StoryRepository(AppDbContext context) : BaseRepository<Story>(context), IStoryRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task RemoveRange(List<Story?> stories)
    {
        if (stories.Count != 0)
        {
            _context1.Stories.RemoveRange(stories!);
            await _context1.SaveChangesAsync();
        }
    }
}