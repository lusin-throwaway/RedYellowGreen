namespace RedYellowGreen.Api.Features.Orders;

public record OrderCreated(Guid OrderId, Guid EquipmentId);

public record OrderCompleted(Guid OrderId);