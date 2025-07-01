using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface IMessageRepository : IBaseRepository<Message>
{
    public Task<List<Message>> GetMessagesAsync(string user1Id, string user2Id);
    public Task DeleteMessageAsync(Message message);
}