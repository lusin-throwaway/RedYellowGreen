using FluentAssertions;
using Integration.Tests.Utilities;
using MassTransit;
using Microsoft.AspNetCore.SignalR.Client;
using RedYellowGreen.Api.Features.Equipment.Models;
using RedYellowGreen.Api.Features.LiveUpdates;

namespace Integration.Tests.Equipment;

[TestClass]
public class EquipmentTests : IntegrationTestBase
{
    [TestMethod]
    public async Task CreateEquipment_CanBeFetched()
    {
        // arrange
        var title = Guid.NewGuid().ToString("N");

        // act
        var equipmentId = await Client.CreateEquipment(title);
        var allEquipment = await Client.GetWorkerViewEquipment();

        // assert
        allEquipment.Should().HaveCount(1);
        var equipment = allEquipment.Single(x => x.Id == equipmentId);

        equipment.Title.Should().Be(title);
        equipment.State.Should().Be(EquipmentState.Red);
    }

    [TestMethod]
    public async Task SetEquipmentState_SameStateAsCurrent_BadRequestException()
    {
        // arrange
        var equipmentId = await Client.CreateEquipment();

        // act
        // assert

        // this is an example of why I added custom exceptions to the ApiClient
        // I think this is a better API than making the http request and catching a flurl exception,
        // then decoding it and so on
        await Client
            .Invoking(x => x.SetEquipmentState(equipmentId, EquipmentState.Red))
            .Should()
            .ThrowAsync<HttpBadRequestException>("can't set the state to current state");
    }

    [TestMethod]
    public async Task SetEquipmentState_ToDifferentState_IsChangedAndAddedToHistory()
    {
        // arrange
        var equipmentId = await Client.CreateEquipment();

        // act
        await Client.SetEquipmentState(equipmentId, EquipmentState.Green);

        // assert
        var allEquipment = await Client.GetWorkerViewEquipment();

        // assert
        allEquipment.Should().HaveCount(1);
        var equipment = allEquipment.Single(x => x.Id == equipmentId);
        equipment.State.Should().Be(EquipmentState.Green);

        var history = await Client.GetEquipmentStateHistory(equipmentId);
        history.Should().HaveCount(2);
    }


    [TestMethod]
    public async Task SetEquipmentState_ToDifferentState_LiveUpdateBroadcastes()
    {
        // arrange
        var equipmentId = await Client.CreateEquipment();
        var connection = Client.CreateLiveUpdatesHubConnection();


        var result = new TaskCompletionSource<ILiveUpdateHub.EquipmentStateChangedEvent>();

        connection.On<ILiveUpdateHub.EquipmentStateChangedEvent>(
            "EquipmentStateChanged",
            msg => result.TrySetResult(msg)
        );

        await connection.StartAsync();

        // act
        var newState = EquipmentState.Green;
        await Client.SetEquipmentState(equipmentId, newState);

        // assert
        await Task.WhenAny(result.Task, Task.Delay(500));
        var receivedPayload = await result.Task;

        receivedPayload
            .Should()
            .BeEquivalentTo(new ILiveUpdateHub.EquipmentStateChangedEvent(equipmentId, newState));
    }
}