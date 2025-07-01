using System.Linq.Expressions;
using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IFollowRepository : IBaseRepository<Follow>
{
    public Task<Follow?> FindSingleAsync(Expression<Func<Follow, bool>> predicate);
}