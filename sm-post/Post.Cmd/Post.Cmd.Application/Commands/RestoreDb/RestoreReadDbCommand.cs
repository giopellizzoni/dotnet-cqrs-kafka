using MediatR;

namespace Post.Cmd.Application.Commands.RestoreDb;

public record RestoreReadDbCommand : IRequest;