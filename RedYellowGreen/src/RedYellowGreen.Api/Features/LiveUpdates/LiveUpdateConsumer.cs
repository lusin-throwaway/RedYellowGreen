using MassTransit;
using Microsoft.AspNetCore.SignalR;
using RedYellowGreen.Api.Features.Equipment;
using RedYellowGreen.Api.Features.Orders;

namespace RedYellowGreen.Api.Features.LiveUpdates;

internal sealed class LiveUpdateConsumer
    : IConsumer<EquipmentStateChanged>, IConsumer<OrderCreated>, IConsumer<OrderCompleted>
{
    private readonly IHubContext<LiveUpdateHub, ILiveUpdateHub> _hubContext;

    public LiveUpdateConsumer(IHubContext<LiveUpdateHub, ILiveUpdateHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task Consume(ConsumeContext<EquipmentStateChanged> context) =>
        _hubContext.Clients.All.EquipmentStateChanged(
            new ILiveUpdateHub.EquipmentStateChangedEvent(context.Message.EquipmentId, context.Message.State));

    public Task Consume(ConsumeContext<OrderCreated> context) =>
        _hubContext.Clients.All.OrderCreated(
            new ILiveUpdateHub.OrderCreatedEvent(context.Message.OrderId, context.Message.EquipmentId));

    public Task Consume(ConsumeContext<OrderCompleted> context) =>
        _hubContext.Clients.All.OrderCompleted(new ILiveUpdateHub.OrderCompletedEvent(context.Message.OrderId));
}