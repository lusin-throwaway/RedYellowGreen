using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Features.Orders.Models;
using RedYellowGreen.Api.Infrastructure.Database;

namespace RedYellowGreen.Api.Features.Orders.Endpoints.Queries;

public class GetOrders : BaseOrdersController
{
    public record Equipment(Guid Id, EquipmentState State);

    public record Result(
        Guid Id,
        string Number,
        DateTime CreatedAt,
        Equipment Equipment
    );

    public async Task<Result[]> Handle(
        [FromServices] AppDbContext dbContext
    ) =>
        await dbContext.Orders
            .Where(order => order.Status != OrderStatus.Done)
            .Select(o =>
                new Result(
                    o.Id,
                    o.OrderNumber,
                    o.CreatedAt,
                    new Equipment(
                        o.Equipment.Id,
                        o.Equipment.CurrentState.State
                    )
                )
            )
            .ToArrayAsync();
}