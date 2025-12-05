using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Infrastructure.Database;

namespace RedYellowGreen.Api.Features.Equipment.Endpoints.Commands;

public class CreateEquipment : BaseEquipmentController
{
    [HttpPost]
    public async Task<Guid> Handle(
        [FromServices] AppDbContext dbContext,
        [FromServices] ILogger<CreateEquipment> logger,
        [FromBody] Request request)
    {
        var equipment = EquipmentEntity.Create(request.Title);
        dbContext.Add(equipment);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Created  Equipment {Title} {Id}", equipment.Title, equipment.Id);
        return equipment.Id;
    }

    public record Request(
        [StringLength(255, MinimumLength = 5)]
        string Title
    );
}