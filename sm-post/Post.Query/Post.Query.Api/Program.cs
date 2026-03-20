using Post.Query.Api.Middlewares;
using Post.Query.Application;
using Post.Query.Infrastructure;
using Post.Query.Infrastructure.DataAcces;

_ = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
    ?? throw new InvalidOperationException("KAFKA_TOPIC environment variable is not set.");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    db.Database.EnsureCreated();
}

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