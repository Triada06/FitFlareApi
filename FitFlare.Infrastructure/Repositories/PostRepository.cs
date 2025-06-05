using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class PostRepository(AppDbContext context) : BaseRepository<Post>(context), IPostRepository
{
    private readonly AppDbContext _context1 = context;

    public override async Task<IEnumerable<Post>> GetAllAsync(int page = 1, int pageSize = 5, bool tracking = true,
        Func<IQueryable<Post>, IQueryable<Post>>? include = null)
    {
        IQueryable<Post> query = _context1.Set<Post>();

        query = tracking ? query : query.AsNoTracking();
        query = query.Where(m => m.Status == "Published");
        if (include != null)
            query = include(query);

        query = query.Skip(page * pageSize - pageSize).Take(pageSize);
        return await query.ToListAsync();
    }
}