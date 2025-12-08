using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RedYellowGreen.Api.Features.Orders.Models;
using RedYellowGreen.Api.Infrastructure.Database;
using RedYellowGreen.Api.Infrastructure.Database.Extensions;

namespace RedYellowGreen.Api.Features.Orders.Endpoints.Commands;

public class CreateOrder : BaseOrdersController
{
    [HttpPost]
    public async Task<Guid> Handle(
        [FromServices] AppDbContext dbContext,
        [FromServices] ILogger<CreateOrder> logger,
        [FromServices] IPublishEndpoint bus,
        [FromBody] Request request
    )
    {
        var equipment = await dbContext.Equipment
            .FindByIdAsync(request.EquipmentId);

        var order = new OrderEntity
        {
            Equipment = equipment,
            OrderNumber = Guid.NewGuid().ToString("N")[..8]
        };
        equipment.Orders.Add(order);

        await dbContext.SaveChangesAsync();
        await bus.Publish(new OrderCreated(order.Id, equipment.Id));
        return order.Id;
    }

    public record Request(Guid EquipmentId);
}