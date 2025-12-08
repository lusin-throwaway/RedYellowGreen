using System.Reflection;
using System.Text.Json.Serialization;
using MassTransit;
using MassTransit.Metadata;
using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Features.LiveUpdates;
using RedYellowGreen.Api.Features.Orders.Models;
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

services.AddMassTransit(x =>
{
    var consumerTypes = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(RegistrationMetadata.IsConsumerOrDefinition)
        .ToArray();

    x.AddConsumers(consumerTypes);
    // in-memory for the sake of the exercise, in a prod setup it would be
    // an actual service bus, e.g. rabbitmq, possibly with an outbox in EF
    x.UsingInMemory((context, cfg) => { cfg.ConfigureEndpoints(context); });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
services.AddOpenApi();

var app = builder.Build();
app.UseCors();
app.MapHub<LiveUpdateHub>("ws/updates");

var runMigrations = Environment.GetEnvironmentVariable("RUN_MIGRATIONS") is "true";
if (runMigrations)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Equipment.Any())
    {
        var equipmentEntity = EquipmentEntity.Create("Equipment 1");
        equipmentEntity.Orders.Add(new OrderEntity
        {
            Equipment = equipmentEntity,
            OrderNumber = "Order1"
        });
        db.Equipment.Add(equipmentEntity);
        db.Equipment.Add(EquipmentEntity.Create("Equipment 2"));
        db.Equipment.Add(EquipmentEntity.Create("Equipment 3"));
        db.SaveChanges();
    }
}


app.MapOpenApi();
app.UseExceptionHandler();
app.MapControllers();
app.Run();