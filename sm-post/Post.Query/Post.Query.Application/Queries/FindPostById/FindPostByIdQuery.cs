using CQRS.Core.Queries;
using MediatR;
using Post.Query.Domain.Entities;

namespace Post.Query.Application.Queries.FindPostById;

public record FindPostByIdQuery(Guid Id) : BaseQuery, IRequest<List<PostEntity>>;