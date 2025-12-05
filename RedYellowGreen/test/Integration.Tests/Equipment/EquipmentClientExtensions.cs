using System.Reflection.Metadata;
using Flurl.Http;
using Integration.Tests.Utilities;
using RedYellowGreen.Api.Features.Equipment.Endpoints.Commands;
using RedYellowGreen.Api.Features.Equipment.Endpoints.Queries;
using RedYellowGreen.Api.Features.Equipment.Models;

namespace Integration.Tests.Equipment;

public static class EquipmentClientExtensions
{
    private const string EquipmentPathRoot = "api/equipment";

    // I add these <see crefs so it's easy to navigate to the implementation

    extension(ApiClient client)
    {
        /// <summary>
        ///     <see cref="CreateEquipment.Handle" />
        /// </summary>
        public Task<Guid> CreateEquipment(string? title = null) =>
            client
                .Request(EquipmentPathRoot)
                .PostJsonAsync(new CreateEquipment.Request(title ?? Guid.NewGuid().ToString("N")))
                .ReceiveJson<Guid>();

        /// <summary>
        ///     <see cref="SetEquipmentState.Handle" />
        /// </summary>
        public Task SetEquipmentState(Guid equipmentId, EquipmentState state) =>
            client
                .Request(EquipmentPathRoot, equipmentId, "state")
                .PostJsonAsync(new SetEquipmentState.Request(state));

        /// <summary>
        ///     <see cref="GetSupervisorViewEquipment.Handle" />
        /// </summary>
        public async Task<GetSupervisorViewEquipment.Result[]> GetSupervisorViewEquipment() =>
            await client
                .Request(EquipmentPathRoot, "supervisor-view")
                .GetJsonAsync<GetSupervisorViewEquipment.Result[]>();

        /// <summary>
        ///     <see cref="GetWorkerViewEquipment.Handle" />
        /// </summary>
        public async Task<GetWorkerViewEquipment.Result[]> GetWorkerViewEquipment() =>
            await client
                .Request(EquipmentPathRoot, "worker-view")
                .GetJsonAsync<GetWorkerViewEquipment.Result[]>();

        /// <summary>
        ///     <see cref="GetEquipmentStateHistory.Handle" />
        /// </summary>
        public async Task<GetEquipmentStateHistory.Result[]> GetEquipmentStateHistory(Guid equipmentId) =>
            await client
                .Request(EquipmentPathRoot, equipmentId, "state-history")
                .GetJsonAsync<GetEquipmentStateHistory.Result[]>();
    }
}