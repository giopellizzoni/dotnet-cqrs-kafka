using MediatR;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.DTOs;
using Post.Cmd.Application.Commands.Posts.DeletePost;
using Post.Cmd.Application.Commands.Posts.EditMessage;
using Post.Cmd.Application.Commands.Posts.LikePost;
using Post.Cmd.Application.Commands.Posts.NewPost;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostsController(IMediator mediator) : ControllerBase
{
    
    [HttpPost]
    public async Task<IActionResult> NewPostAsync(NewPostCommand command)
    {
        await mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, new NewPostResponse
        {
            Id = command.Id,
            Message = "New Post Request completed Successfully"
        });
    }
   
    [HttpPut("likePost/{id}")]
    public async Task<IActionResult> LikePostAsync(Guid id)
    {
        await mediator.Send(new LikePostCommand(id));

        return Ok(new BaseResponse { Message = "Like Post Request Completed Successfully" });
    }

    [HttpPut("editPost/{id}")]
    public async Task<IActionResult> EditMessageAsync(Guid id, EditMessageCommand command)
    {
        command = command with { Id = id };
        await mediator.Send(command);

        return Ok(new BaseResponse { Message = "Edit Message Request Completed Successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemovePostAsync(Guid id, DeletePostCommand command)
    {
        command = command with { Id = id };
        await mediator.Send(command);

        return Ok(new BaseResponse { Message = "Remove Comment Request Completed Successfully" });
    }
}