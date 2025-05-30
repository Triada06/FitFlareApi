using System.Linq.Expressions;
using FitFlare.Application.Helpers.Exceptions;
using FitFlare.Application.Services.Interfaces;
using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Query;

namespace FitFlare.Application.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task UpdateRangeAsync(IEnumerable<Tag> tags) => await tagRepository.UpdateRangeAsync(tags);


    public async Task DeleteAsync(string tagId)
    {
        var tag = await tagRepository.GetByIdAsync(tagId);
        if (tag == null)
            throw new NotFoundException("Tag not found");
        await tagRepository.DeleteAsync(tag);
    }

    public async Task<IEnumerable<Tag?>> FindAsync(Expression<Func<Tag, bool>> predicate,
        Func<IQueryable<Tag>, IIncludableQueryable<Tag, object>>? include = null, bool tracking = true)
    {
        var data = await tagRepository.FindAsync(predicate, include, tracking);
        return data;
    }
}