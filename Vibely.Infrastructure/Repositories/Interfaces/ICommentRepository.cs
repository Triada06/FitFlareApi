using FitFlare.Core.Entities;

namespace FitFlare.Infrastructure.Repositories.Interfaces;

public interface ICommentRepository : IBaseRepository<Comment>
{
    public Task<List<Comment>> GetAllByPostId(string postId, int page,
        int pageSize = 20, bool tracking = true);

    public Task<IEnumerable<Comment>> LoadReplies(string postId, string parentCommentId, int page, int pageSize = 20,
        bool tracking = true);
}