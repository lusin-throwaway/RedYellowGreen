using RedYellowGreen.Api.Features.Equipment.Models;

namespace RedYellowGreen.Api.Features.Equipment;

public record EquipmentStateChanged(Guid EquipmentId, EquipmentState State);