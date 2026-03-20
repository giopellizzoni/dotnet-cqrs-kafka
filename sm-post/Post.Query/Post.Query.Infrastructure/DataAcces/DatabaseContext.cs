using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.DataAcces;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<PostEntity> Posts { get; set; } = null!;
    public DbSet<CommentEntity> Comments { get; set; } = null!;
}
