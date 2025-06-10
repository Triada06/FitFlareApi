using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class StoryViewRepository(AppDbContext context) : IStoryViewRepository
{
    public async Task CreateAsync(StoryView storyView)
    {
        context.StoryViews.Add(storyView);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(StoryView storyView)
    {
        context.StoryViews.Remove(storyView);
        await context.SaveChangesAsync();
    }

    public async Task<List<AppUser>> GetStoryViewers(string storyId, bool tracking = false)
    {
        IQueryable<StoryView> query = context.Set<StoryView>();

        query = tracking ? query : query.AsNoTracking();
        query = query.Where(m => m.StoryId == storyId);
        return await query.Select(m => m.User).ToListAsync();
    }
}