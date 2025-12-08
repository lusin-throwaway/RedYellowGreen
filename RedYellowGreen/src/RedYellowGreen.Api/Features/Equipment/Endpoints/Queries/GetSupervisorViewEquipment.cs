using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Infrastructure.Database;

namespace RedYellowGreen.Api.Features.Equipment.Endpoints.Queries;

public class GetSupervisorViewEquipment : BaseEquipmentController
{
    [HttpGet("supervisor-view")]
    public async Task<Result[]> Handle(
        [FromServices] AppDbContext context
    )
    {
        return await context.Equipment
            .OrderBy(equipment => equipment.Title)
            .Select(equipment => new Result(equipment.Id, equipment.Title, equipment.CurrentState.State))
            .ToArrayAsync();
    }

    public record Result(
        Guid Id,
        string Title,
        EquipmentState State
    );
}