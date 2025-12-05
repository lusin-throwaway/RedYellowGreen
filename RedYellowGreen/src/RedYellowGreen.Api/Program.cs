using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Infrastructure.Database;
using RedYellowGreen.Api.Infrastructure.Database.Interceptors;
using RedYellowGreen.Api.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddSignalR();
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // React dev server
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // needed for SignalR
    });
});

services
    .AddSingleton(TimeProvider.System)
    .AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        };
    })
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddScoped<AuditableFieldsInterceptor>()
    .AddDbContext<AppDbContext>((serviceProvider, options) =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.AddInterceptors(serviceProvider.GetRequiredService<AuditableFieldsInterceptor>());
    })
    ;


services
    .AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
services.AddOpenApi();

var app = builder.Build();
app.UseCors();

var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS") is "true";
if (runMigrations)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.MapOpenApi();
app.UseExceptionHandler();
app.MapControllers();
app.Run();