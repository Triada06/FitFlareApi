using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IStoryRepository : IBaseRepository<Story>
{
    public Task RemoveRange(List<Story?> stories);
}