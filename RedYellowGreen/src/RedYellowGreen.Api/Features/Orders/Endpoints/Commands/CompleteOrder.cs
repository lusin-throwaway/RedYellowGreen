using MassTransit;
using Microsoft.AspNetCore.Mvc;
using RedYellowGreen.Api.Features.Orders.Models;
using RedYellowGreen.Api.Infrastructure;
using RedYellowGreen.Api.Infrastructure.Database;
using RedYellowGreen.Api.Infrastructure.Database.Extensions;

namespace RedYellowGreen.Api.Features.Orders.Endpoints.Commands;

public class CompleteOrder : BaseOrdersController
{
    [HttpPost("{orderId:guid}/complete")]
    public async Task Handle(
        [FromServices] AppDbContext dbContext,
        [FromServices] ILogger<CompleteOrder> logger,
        [FromServices] IPublishEndpoint bus,
        [FromRoute] Guid orderId)
    {
        var order = await dbContext.Orders
            .FindByIdAsync(orderId);

        if (order.Status == OrderStatus.Done)
            throw new BadRequestException($"Order {orderId} is already done");

        order.Status = OrderStatus.Done;

        await dbContext.SaveChangesAsync();
        await bus.Publish(new OrderCompleted(orderId));
    }
}