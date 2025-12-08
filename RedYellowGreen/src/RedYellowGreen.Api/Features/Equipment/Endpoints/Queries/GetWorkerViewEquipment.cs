using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Features.Orders.Models;
using RedYellowGreen.Api.Infrastructure.Database;

namespace RedYellowGreen.Api.Features.Equipment.Endpoints.Queries;

public class GetWorkerViewEquipment : BaseEquipmentController
{
    [HttpGet("worker-view")]
    public async Task<Result[]> Handle(
        [FromServices] AppDbContext context
    )
    {
        var equipment = await context.Equipment
            .Include(equipment => equipment.Orders.Where(order => order.Status != OrderStatus.Done))
            .OrderBy(equipment => equipment.Title)
            .ToArrayAsync();

        return equipment
            .Select(e =>
            {
                var currentOrder = e.Orders
                    .OrderBy(o => o.CreatedAt)
                    .Select(o => new Order(o.Id, o.OrderNumber, o.CreatedAt))
                    .FirstOrDefault();

                var scheduledOrders = e.Orders
                    .Where(o => currentOrder is not null && o.Id != currentOrder.Id)
                    .OrderBy(o => o.CreatedAt)
                    .Select(o => new Order(o.Id, o.OrderNumber, o.CreatedAt))
                    .ToArray();

                return new Result(
                    e.Id,
                    e.Title,
                    e.CurrentState.State,
                    currentOrder,
                    scheduledOrders
                );
            })
            .ToArray();
    }

    public record Order(Guid Id, string Number, DateTime CreatedAt);

    public record Result(
        Guid Id,
        string Title,
        EquipmentState State,
        Order? CurrentOrder,
        Order[] ScheduledOrders
    );
}