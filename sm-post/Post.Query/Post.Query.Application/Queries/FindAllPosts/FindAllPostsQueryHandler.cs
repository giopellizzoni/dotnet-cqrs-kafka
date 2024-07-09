using MediatR;
using Post.Query.Application.DTOs;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Application.Queries.FindAllPosts;

public class FindAllPostsQueryHandler(IPostRepository postRepository)
    : IRequestHandler<FindAllPostsQuery, List<PostEntity>>
{
    private readonly IPostRepository _postRepository = postRepository;

    public async Task<List<PostEntity>> Handle(FindAllPostsQuery request, CancellationToken cancellationToken)
    {
        return await _postRepository.ListAllAsync();
    }
}