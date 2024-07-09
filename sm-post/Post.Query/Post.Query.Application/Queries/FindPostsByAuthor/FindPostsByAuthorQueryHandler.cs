using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Application.Queries.FindPostsByAuthor;

public class FindPostsByAuthorQueryHandler(IPostRepository postRepository)
    : IRequestHandler<FindPostsByAuthorQuery, List<PostEntity>>
{
    public async Task<List<PostEntity>> Handle(FindPostsByAuthorQuery request, 
        CancellationToken cancellationToken) => await postRepository.ListByAuthorAsync(request.Author);
}