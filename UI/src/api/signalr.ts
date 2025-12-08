// src/api/signalr.ts
import * as signalR from "@microsoft/signalr";
import { QueryClient } from "@tanstack/react-query";
import { keys } from "./queries";

export const baseUrl = import.meta.env.VITE_API_BASE_URL;

export function startSignalR(qc: QueryClient) {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5000/ws/updates", { withCredentials: true })
    .withAutomaticReconnect()
    .build();

  // Event fired when any equipment changes state
  connection.on("EquipmentStateChanged", (equipmentId: string) => {
    // Invalidate all relevant caches
    console.log("EquipmentStateChanged received", equipmentId);

    qc.invalidateQueries({ queryKey: keys.supervisorView });
    qc.invalidateQueries({ queryKey: keys.workerView });
    qc.invalidateQueries({ queryKey: keys.stateHistory(equipmentId) });
  });

  connection.on("OrderCreated", (_orderId: string, equipmentId: string) => {
    console.log("OrderCreated received", _orderId, equipmentId);
    // Invalidate all relevant caches
    qc.invalidateQueries({ queryKey: keys.workerView });
    qc.invalidateQueries({ queryKey: keys.stateHistory(equipmentId) });
    qc.invalidateQueries({ queryKey: keys.orders });
  });

  connection.on("OrderCompleted", (_orderId: string) => {
    console.log("OrderCompleted received", _orderId);

    // Invalidate all relevant caches
    qc.invalidateQueries({ queryKey: keys.workerView });
    qc.invalidateQueries({ queryKey: keys.orders });
  });

  connection.start().catch((e) => console.error("SignalR Error", e));

  return connection;
}
