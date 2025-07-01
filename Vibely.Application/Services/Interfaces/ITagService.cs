using System.Linq.Expressions;
using FitFlare.Application.DTOs.Tag;
using FitFlare.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services.Interfaces;

public interface ITagService
{
    public Task UpdateRangeAsync(IEnumerable<Tag> tags);
    public Task DeleteAsync(string tagId);

    public Task<IEnumerable<Tag?>> FindAsync(Expression<Func<Tag, bool>> predicate, Func<IQueryable<Tag>,
        IIncludableQueryable<Tag, object>>? include = null, bool tracking = true);
    public Task<IEnumerable<TagDto?>> SearchAsync(string? searchText);
}