using CQRS.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Post.Cmd.Api.Middlewares;

public class ExceptionHandlers(RequestDelegate next, ILogger<ExceptionHandlers> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AggregateNotFoundException ex)
        {
            logger.LogWarning(ex, "Aggregate not found: {Message}", ex.Message);
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = ex.Message,
            };
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (ConcurrencyException ex)
        {
            logger.LogWarning(ex, "Concurrency conflict: {Message}", ex.Message);
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = ex.Message,
            };
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
            };
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
