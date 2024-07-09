using CQRS.Core.Queries;
using MediatR;
using Post.Query.Domain.Entities;

namespace Post.Query.Application.Queries.FindAllPosts;

public record FindAllPostsQuery: BaseQuery, IRequest<List<PostEntity>>;