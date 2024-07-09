using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Application.Queries.FindPostsWithLikes;

public class FindPostsWithLikesQueryHandler(IPostRepository postRepository)
    : IRequestHandler<FindPostsWithLikesQuery, List<PostEntity>>
{
    public async Task<List<PostEntity>> Handle(FindPostsWithLikesQuery request,
        CancellationToken cancellationToken) => await postRepository.ListWithLikesAsync(request.NumberOfLikes);
}