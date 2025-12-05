using RedYellowGreen.Api.Infrastructure.Database.Models;

namespace RedYellowGreen.Api.Features.Equipment.Models;

internal sealed class EquipmentStateEntity : BaseEntity
{
    public EquipmentState State { get; set; } = EquipmentState.Red;
    public EquipmentEntity Equipment { get; init; } = null!;
    public Guid EquipmentId { get; set; }
}