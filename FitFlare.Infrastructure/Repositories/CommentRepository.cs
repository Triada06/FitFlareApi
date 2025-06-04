using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class CommentRepository(AppDbContext context) : BaseRepository<Comment>(context), ICommentRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task<List<Comment>> GetAllByPostId(string postId, int page, int pageSize = 20,
        bool tracking = true)
    {
        IQueryable<Comment> query = _context1.Set<Comment>();

        query = tracking ? query : query.AsNoTracking();
        query = query.Where(m => m.PostId == postId);
        query = query
            .Include(m => m.Post)
            .Include(m => m.User);

        query = query.Skip(page * pageSize - pageSize).Take(pageSize);
        return await query.ToListAsync();
    }


    public async Task<IEnumerable<Comment>> LoadReplies(string postId, string parentCommentId, int page,
        int pageSize = 20, bool tracking = true)
    {
        IQueryable<Comment> query = _context1.Set<Comment>();

        query = tracking ? query : query.AsNoTracking();
        query = query.Where(m => m.PostId == postId && m.ParentCommentId == parentCommentId );
        query = query
            .Include(m => m.Post)
            .Include(m => m.User);
        query = query.Skip(page * pageSize - pageSize).Take(pageSize);
        return await query.ToListAsync();
    }
}