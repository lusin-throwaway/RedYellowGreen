import { useEffect } from "react";
import "./App.css";
import { startSignalR } from "./api/signalr";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import SupervisorView from "./views/SupervisorView";

const qc = new QueryClient();

function App() {
  useEffect(() => {
    const connection = startSignalR(qc);

    // Cleanup must be synchronous
    return () => {
      connection.stop().catch(() => {}); // ignore errors
    };
  }, []);

  return (
    <QueryClientProvider client={qc}>
      <BrowserRouter>
        <div style={{ display: "flex", gap: 20, padding: 20 }}>
          <Link to="/">Supervisor View</Link>
        </div>

        <Routes>
          <Route path="/" element={<SupervisorView />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
