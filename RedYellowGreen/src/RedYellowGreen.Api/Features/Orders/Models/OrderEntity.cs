using System.ComponentModel.DataAnnotations;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Infrastructure.Database.Models;

namespace RedYellowGreen.Api.Features.Orders.Models;

public enum OrderStatus
{
    Scheduled,
    Done
}

internal sealed class OrderEntity : BaseEntity
{
    public required EquipmentEntity Equipment { get; set; }
    
    [MaxLength(255)]
    public required string OrderNumber { get; set; }
    
    public OrderStatus Status { get; set; } = OrderStatus.Scheduled;
}