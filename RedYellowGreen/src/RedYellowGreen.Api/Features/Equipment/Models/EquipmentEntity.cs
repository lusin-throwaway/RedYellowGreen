using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using RedYellowGreen.Api.Infrastructure;
using RedYellowGreen.Api.Infrastructure.Database.Models;

namespace RedYellowGreen.Api.Features.Equipment.Models;

internal class EquipmentEntity : BaseEntity
{
    private readonly List<EquipmentStateEntity> _states = [];

    private EquipmentEntity()
    {
        // EF and factory method ctor
    }

    [MaxLength(255)]
    public required string Title { get; set; }

    public EquipmentCurrentState CurrentState { get; private set; } = null!;

    public IReadOnlyList<EquipmentStateEntity> States
    {
        get => _states;
        init => _states = value.ToList();
    }

    public void SetState(EquipmentState state)
    {
        if (CurrentState.State == state)
            throw new BadRequestException($"Equipment {Id} is already in state {state}");

        var newState = new EquipmentStateEntity { State = state };
        CurrentState = new(newState.Id, newState.State);
        _states.Add(newState);
    }

    public List<OrderEntity> Orders { get; set; } = [];

    /*
     * Used a factory method here because I wanted to make it impossible
     * to initialize a new EquipmentEntity without a state.
     *
     * And the reason for that is that I don't like having optional properties
     * for things that are not actually optional and would force developers to deal with null checks
     */
    public static EquipmentEntity Create(string title)
    {
        var equipment = new EquipmentEntity
        {
            Title = title
        };

        var initialState = new EquipmentStateEntity();
        equipment._states.Add(initialState);

        equipment.CurrentState = new EquipmentCurrentState(
            initialState.Id,
            initialState.State
        );
        return equipment;
    }

    // owned means this record will be spread into multiple columns on the owning entities row instead of being
    // stored in a different table
    [Owned]
    internal record EquipmentCurrentState(Guid StateId, EquipmentState State);
}