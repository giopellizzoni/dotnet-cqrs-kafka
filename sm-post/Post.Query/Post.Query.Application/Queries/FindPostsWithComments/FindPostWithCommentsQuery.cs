using CQRS.Core.Queries;
using MediatR;
using Post.Query.Domain.Entities;

namespace Post.Query.Application.Queries.FindPostsWithComments;

public record FindPostWithCommentsQuery : BaseQuery, IRequest<List<PostEntity>>;