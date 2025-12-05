using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Infrastructure.Database;
using RedYellowGreen.Api.Infrastructure.Database.Extensions;

namespace RedYellowGreen.Api.Features.Equipment.Endpoints.Commands;

public class SetEquipmentState : BaseEquipmentController
{
    public record Request(EquipmentState State);

    [HttpPut("{equipmentId:guid}/state")]
    public async Task Handle(
        [FromServices] AppDbContext dbContext,
        [FromServices] ILogger<SetEquipmentState> logger,
        [FromServices] IPublishEndpoint bus,
        [FromRoute] Guid equipmentId,
        [FromBody] Request request)
    {
        var equipment = await dbContext.Equipment
            .FindByIdAsync(equipmentId);

        logger.LogInformation("Setting equipment {EquipmentId} state from {OldState} to {NewState}",
            equipmentId,
            equipment.CurrentState.State,
            request.State
        );

        equipment.SetState(request.State);

        await dbContext.SaveChangesAsync();
        await bus.Publish(new EquipmentStateChanged(equipmentId, request.State));
    }
}