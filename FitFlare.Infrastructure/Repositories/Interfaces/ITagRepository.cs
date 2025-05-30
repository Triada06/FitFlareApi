using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface ITagRepository : IBaseRepository<Tag>
{
    public Task UpdateRangeAsync(IEnumerable<Tag> tags);
}