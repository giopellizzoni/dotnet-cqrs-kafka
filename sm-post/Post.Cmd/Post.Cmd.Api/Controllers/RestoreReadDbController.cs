using CQRS.Core.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Application.Commands.RestoreDb;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RestoreReadDbController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> RestoreReadDbAsync(RestoreReadDbCommand command)
    {
        await mediator.Send(command);

        return StatusCode(StatusCodes.Status201Created, new BaseResponse
        {
            Message = "Read database restore request successfully"
        });
    }
}