import Navbar from "../components/Navbar";

export default function Dashboard() {
  return (
    <>
      <Navbar />
      <div style={{ padding: 16 }}>
        <h2>Dashboard</h2>
        <p>Use the navigation to manage employees, attendance, and reports.</p>
      </div>
    </>
  );
}
