using Microsoft.AspNetCore.Mvc;

namespace Post.Query.Api.Middlewares;

public class ExceptionHandlers(RequestDelegate next, ILogger<ExceptionHandlers> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error"
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}