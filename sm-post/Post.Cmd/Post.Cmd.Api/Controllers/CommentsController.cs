using CQRS.Core.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.DTOs;
using Post.Cmd.Application.Commands.Comments.AddComment;
using Post.Cmd.Application.Commands.Comments.EditComment;
using Post.Cmd.Application.Commands.Comments.RemoveComment;
using Post.Cmd.Application.Commands.Posts.NewPost;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CommentsController(IMediator mediator) : ControllerBase
{
    
    [HttpPut("addComment/{id}")]
    public async Task<IActionResult> AddCommentAsync(Guid id, AddCommentCommand command)
    {
        command = command with { Id = id };
        await mediator.Send(command);

        return Ok(new BaseResponse { Message = "Add Comment Request Completed Successfully" });
    }


    [HttpPut("editComment/{id}")]
    public async Task<IActionResult> EditCommentAsync(Guid id, EditCommentCommand command)
    {
        command = command with { Id = id };
        await mediator.Send(command);

        return Ok(new BaseResponse { Message = "Edit Comment Request Completed Successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveCommentAsync(Guid id, RemoveCommentCommand command)
    {
        command = command with { Id = id };
        await mediator.Send(command);

        return Ok(new BaseResponse { Message = "Remove Comment Request Completed Successfully" });
    }
}