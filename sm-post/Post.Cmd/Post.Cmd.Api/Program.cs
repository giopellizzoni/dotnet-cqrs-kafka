using Post.Cmd.Api.Middlewares;
using Post.Cmd.Application;
using Post.Cmd.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (Environment.GetEnvironmentVariable("KAFKA_TOPIC") is null)
    throw new InvalidOperationException("KAFKA_TOPIC environment variable is not set.");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlers>();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
