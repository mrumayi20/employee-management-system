import { Link, useNavigate } from "react-router-dom";
import { clearToken } from "../auth/auth";

export default function Navbar() {
  const nav = useNavigate();

  function logout() {
    clearToken();
    nav("/login");
  }

  return (
    <div
      style={{
        display: "flex",
        gap: 12,
        padding: 12,
        borderBottom: "1px solid #ddd",
        alignItems: "center",
        justifyContent: "space-between",
      }}
    >
      <div style={{ display: "flex", gap: 12, alignItems: "center" }}>
        <strong>EMS</strong>
        <Link to="/dashboard">Dashboard</Link>
        <Link to="/employees">Employees</Link>
        <Link to="/departments">Departments</Link>
        <Link to="/attendance">Attendance</Link>
        <Link to="/reports">Reports</Link>
      </div>

      <button onClick={logout}>Logout</button>
    </div>
  );
}
