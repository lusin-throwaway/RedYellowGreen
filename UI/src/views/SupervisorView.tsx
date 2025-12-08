import { useState } from "react";
import {
  useSupervisorView,
  useEquipmentStateHistory,
  useCreateOrder,
  useGetOrders,
} from "../api/queries";
import type { GetOrdersResult } from "../api/types";

export default function SupervisorView() {
  const { data: equipment, isLoading } = useSupervisorView();
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const { data: orders, isLoading: ordersLoading } = useGetOrders();
  if (isLoading || !equipment || ordersLoading || !orders)
    return <div>Loading...</div>;

  return (
    <div style={{ padding: 20 }}>
      <h1>Supervisor View</h1>
      <h2>Equipment</h2>

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

      <h2 style={{ marginTop: 40 }}>Orders</h2>
      <table border={1} cellPadding={10} style={{ width: "100%" }}>
        <thead>
          <tr>
            <th>Number</th>
            <th>Created At</th>
            <th>Equipment</th>
            <th>Equipment State</th>
          </tr>
        </thead>
        <tbody>
          {orders?.map((o: GetOrdersResult) => (
            <tr key={o.id}>
              <td>{o.number}</td>
              <td>{new Date(o.createdAt).toLocaleString()}</td>
              <td>{o.equipment.title}</td>
              <td>{o.equipment.state}</td>
            </tr>
          ))}
        </tbody>
      </table>

      {/* SIDE PANEL */}
      {selectedId && (
        <SidePanel
          equipmentId={selectedId}
          onClose={() => setSelectedId(null)}
          equipmentTitle={
            equipment.find((e) => e.id === selectedId)?.title || ""
          }
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
  equipmentTitle,
  onClose,
}: {
  equipmentId: string;
  equipmentTitle: string;
  onClose: () => void;
}) {
  const { data: history } = useEquipmentStateHistory(equipmentId);
  const { mutate: createOrder } = useCreateOrder(equipmentId);

  if (!history) return null;

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
      <p>Title: {equipmentTitle}</p>

      <button onClick={() => createOrder()} style={{ marginTop: 20 }}>
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
