import { api } from "./client";
import type {
  EquipmentState,
  GetSupervisorViewEquipmentResult,
  GetWorkerViewEquipmentResult,
} from "./types";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

export const keys = {
  supervisorView: ["supervisor-view"],
  workerView: ["worker-view"],
  stateHistory: (id: string) => ["state-history", id],
  equipment: (id: string) => ["equipment", id],
  orders: ["orders"],
};

export function useSupervisorView() {
  return useQuery({
    queryKey: keys.supervisorView,
    queryFn: fetchSupervisorView,
  });
}

export function useWorkerView() {
  return useQuery({
    queryKey: keys.workerView,
    queryFn: fetchWorkerView,
  });
}

export function useEquipmentStateHistory(equipmentId?: string) {
  return useQuery({
    queryKey: equipmentId
      ? keys.stateHistory(equipmentId)
      : ["state-history", "none"],
    queryFn: () => fetchEquipmentStateHistory(equipmentId!),
    enabled: !!equipmentId, // only fetch when id exists
  });
}

// -------------------------------------------------------
// Mutations
// -------------------------------------------------------
export function useSetEquipmentState() {
  const qc = useQueryClient();

  return useMutation({
    mutationFn: ({ id, state }: { id: string; state: EquipmentState }) =>
      setEquipmentState(id, state),

    onSuccess: (_, { id }) => {
      // Update views that show equipment state
      qc.invalidateQueries({ queryKey: keys.supervisorView });
      qc.invalidateQueries({ queryKey: keys.workerView });
      qc.invalidateQueries({ queryKey: keys.stateHistory(id) });
    },
  });
}

export function useCreateOrder() {
  const qc = useQueryClient();

  return useMutation({
    mutationFn: createOrder,
    onSuccess: () => {
      // Invalidate whichever views show orders
      qc.invalidateQueries({ queryKey: keys.orders });
      qc.invalidateQueries({ queryKey: keys.supervisorView });
      qc.invalidateQueries({ queryKey: keys.workerView });
    },
  });
}

export function useCompleteOrder() {
  const qc = useQueryClient();

  return useMutation({
    mutationFn: ({ orderId }: { orderId: string }) => completeOrder(orderId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: keys.orders });
      qc.invalidateQueries({ queryKey: keys.supervisorView });
      qc.invalidateQueries({ queryKey: keys.workerView });
    },
  });
}

async function fetchSupervisorView(): Promise<
  GetSupervisorViewEquipmentResult[]
> {
  const { data } = await api.get("/api/supervisor-view");
  return data;
}

async function fetchWorkerView(): Promise<GetWorkerViewEquipmentResult[]> {
  const { data } = await api.get("/api/worker-view");
  return data;
}

async function fetchEquipmentStateHistory(
  equipmentId: string
): Promise<GetWorkerViewEquipmentResult[]> {
  const { data } = await api.get(
    `/api/worker-view/${equipmentId}/state-history`
  );
  return data;
}

async function setEquipmentState(equipmentId: string, state: EquipmentState) {
  await api.put(`/api/equipment/${equipmentId}/state`, {
    state,
  });
}

async function createOrder(): Promise<string> {
  const { data } = await api.post(`/api/orders`);
  return data;
}

async function completeOrder(orderId: string) {
  await api.post(`/api/orders/${orderId}/complete`);
}
