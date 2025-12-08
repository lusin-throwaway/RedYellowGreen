// src/views/WorkerView.tsx
import {
  useWorkerView,
  useCompleteOrder,
  useSetEquipmentState,
} from "../api/queries";
import type {
  EquipmentState,
  GetWorkerViewEquipmentResult,
} from "../api/types";

export default function WorkerView() {
  const { data: equipment, isLoading } = useWorkerView();
  const { mutate: setEquipmentStateMutation } = useSetEquipmentState();
  const { mutate: completeOrder } = useCompleteOrder();

  if (isLoading) return <div>Loading...</div>;

  return (
    <div style={{ padding: 20 }}>
      <h1>Worker View</h1>

      <table
        border={1}
        cellPadding={10}
        style={{ marginTop: 20, width: "100%" }}
      >
        <thead>
          <tr>
            <th>Name</th>
            <th>State</th>
            <th>Current Order</th>
            <th>Scheduled Orders</th>
          </tr>
        </thead>

        <tbody>
          {equipment?.map((e: GetWorkerViewEquipmentResult) => (
            <tr key={e.id}>
              <td>{e.title}</td>
              <td>
                <select
                  value={e.state}
                  onChange={(ev) =>
                    setEquipmentStateMutation({
                      id: e.id,
                      state: ev.target.value as EquipmentState,
                    })
                  }
                >
                  <option value="Red">Red</option>
                  <option value="Yellow">Yellow</option>
                  <option value="Green">Green</option>
                </select>
              </td>

              {/* Current Order with Complete button */}
              <td>
                {e.currentOrder ? (
                  <div
                    style={{ display: "flex", alignItems: "center", gap: 10 }}
                  >
                    <span>{e.currentOrder.number}</span>
                    <button
                      disabled={!e.currentOrder}
                      onClick={() =>
                        completeOrder({ orderId: e.currentOrder!.id })
                      }
                    >
                      Complete
                    </button>
                  </div>
                ) : (
                  "-"
                )}
              </td>

              {/* Scheduled Orders as bullet list */}
              <td>
                {e.scheduledOrders.length > 0 ? (
                  <ul style={{ margin: 0, paddingLeft: 20 }}>
                    {e.scheduledOrders.map((o) => (
                      <li key={o.id}>
                        {o.number}
                      </li>
                    ))}
                  </ul>
                ) : (
                  "-"
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
