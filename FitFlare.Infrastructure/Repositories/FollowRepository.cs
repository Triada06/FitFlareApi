using System.Linq.Expressions;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class FollowRepository(AppDbContext context) : BaseRepository<Follow>(context), IFollowRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task<Follow?> FindSingleAsync(Expression<Func<Follow, bool>> predicate)
        => await _context1.Follows.Where(predicate).FirstOrDefaultAsync();
}