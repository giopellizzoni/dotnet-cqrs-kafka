namespace Post.Query.Domain.Repositories;

public interface IBaseRepository<T>
{
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<T?> GetByIdAsync(Guid id);
    
}