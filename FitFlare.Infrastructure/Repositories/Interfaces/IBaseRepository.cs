using System.Linq.Expressions;
using FitFlare.Core;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class, IBaseEntity
{
    public Task<bool> CreateAsync(T entity);
    public Task<bool> UpdateAsync(T entity);
    public Task<bool> DeleteAsync(T entity);

    public Task<T?> GetByIdAsync(string id, Func<IQueryable<T>,
        IIncludableQueryable<T, object>>? include = null, bool tracking = true);

    public Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = 5, bool tracking = true,
        Func<IQueryable<T>, IQueryable<T>>? include = null);

    public Task<IEnumerable<T?>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>,
        IIncludableQueryable<T, object>>? include = null, bool tracking = true);

    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
}