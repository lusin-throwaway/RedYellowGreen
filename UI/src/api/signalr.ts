// src/api/signalr.ts
import * as signalR from "@microsoft/signalr";
import { QueryClient } from "@tanstack/react-query";
import { keys } from "./queries";

export const baseUrl = import.meta.env.VITE_API_BASE_URL;

export function startSignalR(qc: QueryClient) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${baseUrl}/ws/updates`)
    .withAutomaticReconnect()
    .build();

  // Event fired when any equipment changes state
  connection.on("EquipmentStateChanged", (equipmentId: string) => {
    // Invalidate all relevant caches
    qc.invalidateQueries({ queryKey: keys.supervisorView });
    qc.invalidateQueries({ queryKey: keys.workerView });
    qc.invalidateQueries({ queryKey: keys.stateHistory(equipmentId) });
  });

  connection.on("OrderCreated", (_orderId: string, equipmentId: string) => {
    // Invalidate all relevant caches
    qc.invalidateQueries({ queryKey: keys.workerView });
    qc.invalidateQueries({ queryKey: keys.stateHistory(equipmentId) });
    qc.invalidateQueries({ queryKey: keys.orders });
  });

  connection.on("OrderCompleted", (_orderId: string) => {
    // Invalidate all relevant caches
    qc.invalidateQueries({ queryKey: keys.workerView });
    qc.invalidateQueries({ queryKey: keys.orders });
  });

  connection.start().catch((e) => console.error("SignalR Error", e));

  return connection;
}
