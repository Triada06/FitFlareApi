using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IBanRepository : IBaseRepository<Ban>
{
    public Task RemoveRange(List<Ban> bans);
}