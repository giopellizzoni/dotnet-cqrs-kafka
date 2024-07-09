using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Application.DTOs;
using Post.Query.Application.Queries.FindAllPosts;
using Post.Query.Application.Queries.FindPostById;
using Post.Query.Application.Queries.FindPostsByAuthor;
using Post.Query.Application.Queries.FindPostsWithComments;
using Post.Query.Application.Queries.FindPostsWithLikes;

namespace Post.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PostLookupController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllPostsAsync()
    {
        var posts = await mediator.Send(new FindAllPostsQuery());
        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        var count = posts.Count;
        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}!"
        });
    }

    [HttpGet("byId/{id}")]
    public async Task<IActionResult> GetByPostIdAsync(Guid id)
    {
        var posts = await mediator.Send(new FindPostByIdQuery(id));
        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        var count = posts.Count;
        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned post!"
        });
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<IActionResult> GetByAuthorAsync(string author)
    {
        var posts = await mediator.Send(new FindPostsByAuthorQuery(author));
        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned posts!"
        });
    }


    [HttpGet("withComments")]
    public async Task<IActionResult> GetWithCommentsAsync()
    {
        var posts = await mediator.Send(new FindPostWithCommentsQuery());
        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        var count = posts.Count;
        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned posts!"
        });
    }

    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<IActionResult> GetWithLikesAsync(int numberOfLikes)
    {
        var posts = await mediator.Send(new FindPostsWithLikesQuery(numberOfLikes));
        if (posts == null || !posts.Any())
        {
            return NoContent();
        }

        var count = posts.Count;
        return Ok(new PostLookupResponse
        {
            Posts = posts,
            Message = $"Successfully returned posts!"
        });
    }
}