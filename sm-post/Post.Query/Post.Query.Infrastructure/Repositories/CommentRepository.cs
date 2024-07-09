using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAcces;

namespace Post.Query.Infrastructure.Repositories;

public class CommentRepository(DatabaseContextFactory contextFactory)
    : BaseRepository<CommentEntity>(contextFactory), ICommentRepository
{
    public override async Task<CommentEntity?> GetByIdAsync(Guid id)
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == id);
    }
}