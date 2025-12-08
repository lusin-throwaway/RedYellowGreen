// src/api/types.ts

export type EquipmentState = "Red" | "Yellow" | "Green";

export type GetSupervisorViewEquipmentResult = {
  id: string;
  title: string;
  state: EquipmentState;
};

export type GetEquipmentStateHistoryResult = {
  state: EquipmentState;
  createdAt: string;
};

export type GetWorkerViewEquipmentResult = {
  id: string;
  title: string;
  state: EquipmentState;
  currentOrder: GetWorkerViewEquipmentOrder | null;
  scheduledOrders: GetWorkerViewEquipmentOrder[];
};

export type GetWorkerViewEquipmentOrder = {
  id: string;
  number: string;
  state: EquipmentState;
  createdAt: string;
};

export type GetOrdersResult = {
  id: string;
  number: string;
  createdAt: string;
  equipment: GetOrdersEquipment;
};

export type GetOrdersEquipment = {
  id: string;
  title: string;
  state: EquipmentState;
};
