using CQRS.Core.Queries;
using MediatR;
using Post.Query.Domain.Entities;

namespace Post.Query.Application.Queries.FindPostsByAuthor;

public record FindPostsByAuthorQuery(string Author) : BaseQuery, IRequest<List<PostEntity>>;