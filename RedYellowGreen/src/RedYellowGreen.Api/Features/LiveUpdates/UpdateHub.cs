using Microsoft.AspNetCore.SignalR;
using RedYellowGreen.Api.Features.Equipment.Models;

namespace RedYellowGreen.Api.Features.LiveUpdates;

public interface ILiveUpdateHub
{
    public record EquipmentStateChangedEvent(Guid Id, EquipmentState State);

    Task EquipmentStateChanged(EquipmentStateChangedEvent @event);

    public record OrderCreatedEvent(Guid OrderId, Guid EquipmentId);

    Task OrderCreated(OrderCreatedEvent @event);

    public record OrderCompletedEvent(Guid OrderId);

    Task OrderCompleted(OrderCompletedEvent @event);
}

public class LiveUpdateHub : Hub<ILiveUpdateHub>
{
}