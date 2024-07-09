using Post.Query.Domain.Entities;

namespace Post.Query.Domain.Repositories;

public interface IPostRepository : IBaseRepository<PostEntity>
{
    Task<List<PostEntity>> ListAllAsync();
    Task<List<PostEntity>> ListByAuthorAsync(string author);
    Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes);
    Task<List<PostEntity>> ListWithCommentsAsync();
}