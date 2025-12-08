using Microsoft.AspNetCore.SignalR;
using RedYellowGreen.Api.Features.Equipment.Models;

namespace RedYellowGreen.Api.Features.LiveUpdates;

public interface ILiveUpdateHub
{
    Task EquipmentStateChanged(EquipmentStateChangedEvent @event);

    Task OrderCreated(OrderCreatedEvent @event);

    Task OrderCompleted(OrderCompletedEvent @event);

    public record EquipmentStateChangedEvent(Guid Id, EquipmentState State);

    public record OrderCreatedEvent(Guid OrderId, Guid EquipmentId);

    public record OrderCompletedEvent(Guid OrderId);
}

public class LiveUpdateHub : Hub<ILiveUpdateHub>
{
}