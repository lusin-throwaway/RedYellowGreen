using Microsoft.AspNetCore.SignalR;
using RedYellowGreen.Api.Features.Equipment.Models;

namespace RedYellowGreen.Api.Features.LiveUpdates;

public interface ILiveUpdateHub
{
    Task EquipmentStateChanged(Guid equipmentId, EquipmentState state);
    Task OrderCreated(Guid orderId, Guid equipmentId);
    Task OrderCompleted(Guid orderId);
}

public class LiveUpdateHub : Hub<ILiveUpdateHub>
{
}