import { useEffect } from "react";
import "./App.css";
import { startSignalR } from "./api/signalr";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import SupervisorView from "./views/SupervisorView";
import WorkerView from "./views/WorkerView";

const queryClient = new QueryClient();

function App() {
  useEffect(() => {
    const connection = startSignalR(queryClient);

    return () => {
      connection.stop().catch(() => {}); 
    };
  }, []);

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        {/* Navigation */}
        <nav style={{ display: "flex", gap: 20, padding: 20 }}>
          <Link to="/">Supervisor View</Link>
          <Link to="/worker">Worker View</Link>
        </nav>

        {/* Routes */}
        <Routes>
          <Route path="/" element={<SupervisorView />} />
          <Route path="/worker" element={<WorkerView />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
}

export default App;
