using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAcces;

namespace Post.Query.Infrastructure.Repositories;

public abstract class BaseRepository<T>: IBaseRepository<T> where T: IEntity
{
    protected readonly DatabaseContextFactory _contextFactory;

    public BaseRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }


    public async Task CreateAsync(T? entity)
    {
        await using var context = _contextFactory.CreateDbContext();
        context.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await using var context = _contextFactory.CreateDbContext();
        context.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var context = _contextFactory.CreateDbContext();
        var entity = await GetByIdAsync(id);
        if (entity == null) return;

        context.Remove(entity);
        await context.SaveChangesAsync();
    }

    public abstract Task<T?> GetByIdAsync(Guid id);
}