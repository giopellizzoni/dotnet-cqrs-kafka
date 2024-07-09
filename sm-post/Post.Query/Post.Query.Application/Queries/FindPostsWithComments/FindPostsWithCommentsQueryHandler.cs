using MediatR;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Application.Queries.FindPostsWithComments;

public class FindPostsWithCommentsQueryHandler(IPostRepository postRepository): IRequestHandler<FindPostWithCommentsQuery, List<PostEntity>>
{
    public async Task<List<PostEntity>> Handle(FindPostWithCommentsQuery request, 
        CancellationToken cancellationToken) => await postRepository.ListWithCommentsAsync();
}