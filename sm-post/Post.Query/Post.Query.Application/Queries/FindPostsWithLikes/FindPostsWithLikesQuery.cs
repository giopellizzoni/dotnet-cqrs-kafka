using CQRS.Core.Queries;
using MediatR;
using Post.Query.Domain.Entities;

namespace Post.Query.Application.Queries.FindPostsWithLikes;

public record FindPostsWithLikesQuery(int NumberOfLikes) : BaseQuery, IRequest<List<PostEntity>>;