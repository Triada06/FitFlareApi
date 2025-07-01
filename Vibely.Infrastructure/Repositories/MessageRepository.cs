using FitFlare.Core.Entities;
using FitFlare.Infrastructure.Data;
using FitFlare.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitFlare.Infrastructure.Repositories;

public class MessageRepository(AppDbContext context) : BaseRepository<Message>(context), IMessageRepository
{
    private readonly AppDbContext _context1 = context;

    public async Task<List<Message>> GetMessagesAsync(string user1Id, string user2Id) => await _context1.Messages
        .Where(m =>
            (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
            (m.SenderId == user2Id && m.ReceiverId == user1Id))
        .OrderBy(m => m.CreatedAt)
        .ToListAsync();

    public async Task DeleteMessageAsync(Message message)
    {
        _context1.Messages.Remove(message);
        await _context1.SaveChangesAsync();
    }
}