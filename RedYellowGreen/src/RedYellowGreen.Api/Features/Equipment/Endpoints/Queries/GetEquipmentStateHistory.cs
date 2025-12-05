using Microsoft.AspNetCore.Mvc;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Infrastructure.Database;
using RedYellowGreen.Api.Infrastructure.Database.Extensions;

namespace RedYellowGreen.Api.Features.Equipment.Endpoints.Queries;

public class GetEquipmentStateHistory : BaseEquipmentController
{
    public record Result(
        EquipmentState State,
        DateTime CreatedAt
    );

    [HttpGet("{equipmentId:guid}/state-history")]
    public async Task<Result[]> Handle(
        [FromServices] AppDbContext context,
        [FromRoute] Guid equipmentId
    ) =>
        await context.Equipment
            .Where(equipment => equipment.Id == equipmentId)
            .Select(equipment => equipment.States.Select(state => new Result(state.State, state.CreatedAt)).ToArray())
            .SingleOrNotFoundAsync();
}