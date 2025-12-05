using System.ComponentModel.DataAnnotations;
using RedYellowGreen.Api.Infrastructure.Database.Models;

namespace RedYellowGreen.Api.Features.Equipment.Models;

internal sealed class OrderEntity : BaseEntity
{
    public required EquipmentEntity Equipment { get; set; }

    [MaxLength(255)]
    public required string OrderNumber { get; set; }
}