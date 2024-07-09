using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Application.Queries.FindPostById;

public class FindPostByIdQueryHandler(IPostRepository postRepository)
    : IRequestHandler<FindPostByIdQuery, List<PostEntity>>
{
    public async Task<List<PostEntity>> Handle(FindPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await postRepository.GetByIdAsync(request.Id);
        if (post != null) return [post];
        return [];
    }
}