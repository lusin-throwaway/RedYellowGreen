import { useState } from "react";
import {
  useSupervisorView,
  useEquipmentStateHistory,
  useCreateOrder,
} from "../api/queries";
import { type GetEquipmentStateHistoryResult } from "../api/types";

export default function SupervisorView() {
  const { data: equipment, isLoading } = useSupervisorView();
  const [selectedId, setSelectedId] = useState<string | null>(null);

  const { data: history } = useEquipmentStateHistory(selectedId || undefined);
  const createOrderMutation = useCreateOrder();

  if (isLoading) return <div>Loading...</div>;

  return (
    <div style={{ padding: 20 }}>
      <h1>Supervisor View</h1>

      {/* EQUIPMENT TABLE */}
      <table border={1} cellPadding={10} style={{ marginTop: 20 }}>
        <thead>
          <tr>
            <th>Title</th>
            <th>State</th>
            <th></th>
          </tr>
        </thead>

        <tbody>
          {equipment?.map((e) => (
            <tr key={e.id}>
              <td>{e.title}</td>
              <td>{e.state}</td>
              <td>
                <button onClick={() => setSelectedId(e.id)}>Details</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* SIDE PANEL */}
      {selectedId && (
        <SidePanel
          equipmentId={selectedId}
          history={history || []}
          onClose={() => setSelectedId(null)}
          onScheduleOrder={() => {
            createOrderMutation.mutate();
          }}
        />
      )}
    </div>
  );
}

// --------------------------------------------------
// Side Drawer Component
// --------------------------------------------------
function SidePanel({
  equipmentId,
  history,
  onScheduleOrder,
  onClose,
}: {
  equipmentId: string;
  history: GetEquipmentStateHistoryResult[];
  onScheduleOrder: () => void;
  onClose: () => void;
}) {
  return (
    <div
      style={{
        position: "fixed",
        top: 0,
        right: 0,
        width: 350,
        height: "100%",
        background: "#fff",
        boxShadow: "0 0 10px rgba(0,0,0,0.2)",
        padding: 20,
      }}
    >
      <button onClick={onClose}>Close</button>

      <h2>Equipment Details</h2>
      <p>ID: {equipmentId}</p>

      <button onClick={onScheduleOrder} style={{ marginTop: 20 }}>
        Schedule New Order
      </button>

      <h3 style={{ marginTop: 20 }}>State History</h3>

      <table border={1} cellPadding={10}>
        <thead>
          <tr>
            <th>State</th>
            <th>Timestamp</th>
          </tr>
        </thead>

        <tbody>
          {history.map((h, i) => (
            <tr key={i}>
              <td>{h.state}</td>
              <td>{new Date(h.createdAt).toLocaleString()}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
