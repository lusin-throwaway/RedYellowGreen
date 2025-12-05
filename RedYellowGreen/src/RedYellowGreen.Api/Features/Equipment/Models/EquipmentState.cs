namespace RedYellowGreen.Api.Features.Equipment.Models;

// public because it's exposed in the API
public enum EquipmentState
{
    // standing still
    Red,

    // starting up/winding down
    Yellow,

    // Producing normally
    Green,
}