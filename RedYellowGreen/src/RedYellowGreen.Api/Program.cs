using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Infrastructure.Database;
using RedYellowGreen.Api.Infrastructure.Database.Interceptors;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.

services
    .AddSingleton(TimeProvider.System)
    .AddScoped<AuditableFieldsInterceptor>()
    .AddDbContext<AppDbContext>((serviceProvider, options) =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        options.AddInterceptors(serviceProvider.GetRequiredService<AuditableFieldsInterceptor>());
    })
    ;


services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();