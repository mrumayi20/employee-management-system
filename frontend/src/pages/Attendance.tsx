import { useEffect, useMemo, useState } from "react";
import api from "../api/axios";
import Navbar from "../components/Navbar";
import type { Attendance, Employee } from "../types/models";

type AttendanceForm = {
  employeeId: string;
  date: string; // yyyy-mm-dd
  status: number; // 1 Present, 2 Absent, 3 HalfDay, 4 Leave
  checkIn: string;
  checkOut: string;
};

const STATUS_OPTIONS = [
  { value: 1, label: "Present" },
  { value: 2, label: "Absent" },
  { value: 3, label: "HalfDay" },
  { value: 4, label: "Leave" },
];

export default function AttendancePage() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [rows, setRows] = useState<Attendance[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Filters
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");
  const [filterEmployeeId, setFilterEmployeeId] = useState("");

  // Form
  const [form, setForm] = useState<AttendanceForm>({
    employeeId: "",
    date: "",
    status: 1,
    checkIn: "",
    checkOut: "",
  });

  const statusLabel = useMemo(
    () => STATUS_OPTIONS.find((s) => s.value === form.status)?.label,
    [form.status],
  );

  const isTimeAllowed = useMemo(() => {
    return form.status === 1 || form.status === 3; // Present/HalfDay
  }, [form.status]);

  async function loadEmployees() {
    const res = await api.get<Employee[]>("/employees");
    setEmployees(res.data);
    if (!form.employeeId && res.data.length > 0) {
      setForm((f) => ({ ...f, employeeId: res.data[0].id }));
    }
    if (!filterEmployeeId && res.data.length > 0) {
      setFilterEmployeeId(""); // keep as "All"
    }
  }

  async function loadAttendance() {
    setError(null);
    setLoading(true);
    try {
      const params: any = {};
      if (from) params.from = from;
      if (to) params.to = to;
      if (filterEmployeeId) params.employeeId = filterEmployeeId;

      const res = await api.get<Attendance[]>("/attendance", { params });
      setRows(res.data);
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to load attendance.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    (async () => {
      try {
        await loadEmployees();
        await loadAttendance();
      } catch (e: any) {
        setError(e?.response?.data?.error ?? "Failed to load data.");
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function applyFilters() {
    await loadAttendance();
  }

  async function markAttendance(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!form.employeeId) return setError("Select an employee.");
    if (!form.date) return setError("Select a date.");

    // if present/halfday -> validate times (optional but consistent)
    if (isTimeAllowed) {
      if (form.checkIn && form.checkOut && form.checkOut < form.checkIn) {
        return setError("Check-out cannot be earlier than check-in.");
      }
    }

    try {
      await api.post("/attendance", {
        employeeId: form.employeeId,
        date: form.date, // DateOnly in API works with YYYY-MM-DD
        status: form.status,
        checkIn: isTimeAllowed && form.checkIn ? `${form.checkIn}:00` : null,
        checkOut: isTimeAllowed && form.checkOut ? `${form.checkOut}:00` : null,
      });

      // reset date & times, keep employee
      setForm((f) => ({
        ...f,
        date: "",
        checkIn: "",
        checkOut: "",
      }));

      await loadAttendance();
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to mark attendance.");
    }
  }

  async function deleteAttendance(id: string) {
    const ok = confirm("Delete this attendance record?");
    if (!ok) return;

    setError(null);
    try {
      await api.delete(`/attendance/${id}`);
      setRows((prev) => prev.filter((x) => x.id !== id));
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to delete attendance.");
    }
  }

  return (
    <>
      <Navbar />
      <div style={{ padding: 16, display: "grid", gap: 16 }}>
        <h2>Attendance</h2>

        {error && (
          <div
            style={{
              color: "crimson",
              border: "1px solid #f2b8b8",
              padding: 10,
            }}
          >
            {error}
          </div>
        )}

        {/* Mark Attendance */}
        <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
          <h3 style={{ marginTop: 0 }}>Mark Attendance</h3>

          {employees.length === 0 ? (
            <p>No employees found. Create employees first.</p>
          ) : (
            <form
              onSubmit={markAttendance}
              style={{
                display: "grid",
                gridTemplateColumns: "repeat(3, minmax(220px, 1fr))",
                gap: 12,
              }}
            >
              <div>
                <label style={{ display: "block", marginBottom: 4 }}>
                  Employee
                </label>
                <select
                  value={form.employeeId}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, employeeId: e.target.value }))
                  }
                  style={{ width: "100%", padding: 8 }}
                >
                  {employees.map((emp) => (
                    <option key={emp.id} value={emp.id}>
                      {emp.employeeCode} - {emp.fullName}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label style={{ display: "block", marginBottom: 4 }}>
                  Date
                </label>
                <input
                  type="date"
                  value={form.date}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, date: e.target.value }))
                  }
                  style={{ width: "100%", padding: 8 }}
                />
              </div>

              <div>
                <label style={{ display: "block", marginBottom: 4 }}>
                  Status
                </label>
                <select
                  value={form.status}
                  onChange={(e) => {
                    const v = Number(e.target.value);
                    setForm((f) => ({
                      ...f,
                      status: v,
                      // clear times automatically if not allowed
                      checkIn: v === 1 || v === 3 ? f.checkIn : "",
                      checkOut: v === 1 || v === 3 ? f.checkOut : "",
                    }));
                  }}
                  style={{ width: "100%", padding: 8 }}
                >
                  {STATUS_OPTIONS.map((s) => (
                    <option key={s.value} value={s.value}>
                      {s.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label style={{ display: "block", marginBottom: 4 }}>
                  Check In {isTimeAllowed ? "" : "(disabled)"}
                </label>
                <input
                  type="time"
                  value={form.checkIn}
                  disabled={!isTimeAllowed}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, checkIn: e.target.value }))
                  }
                  style={{ width: "100%", padding: 8 }}
                />
              </div>

              <div>
                <label style={{ display: "block", marginBottom: 4 }}>
                  Check Out {isTimeAllowed ? "" : "(disabled)"}
                </label>
                <input
                  type="time"
                  value={form.checkOut}
                  disabled={!isTimeAllowed}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, checkOut: e.target.value }))
                  }
                  style={{ width: "100%", padding: 8 }}
                />
              </div>

              <div style={{ display: "flex", alignItems: "end" }}>
                <button style={{ padding: "8px 12px" }}>
                  Save ({statusLabel})
                </button>
              </div>
            </form>
          )}
        </div>

        {/* Filters */}
        <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
          <h3 style={{ marginTop: 0 }}>Filters</h3>

          <div
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(4, minmax(200px, 1fr))",
              gap: 12,
              alignItems: "end",
            }}
          >
            <div>
              <label style={{ display: "block", marginBottom: 4 }}>From</label>
              <input
                type="date"
                value={from}
                onChange={(e) => setFrom(e.target.value)}
                style={{ width: "100%", padding: 8 }}
              />
            </div>

            <div>
              <label style={{ display: "block", marginBottom: 4 }}>To</label>
              <input
                type="date"
                value={to}
                onChange={(e) => setTo(e.target.value)}
                style={{ width: "100%", padding: 8 }}
              />
            </div>

            <div>
              <label style={{ display: "block", marginBottom: 4 }}>
                Employee
              </label>
              <select
                value={filterEmployeeId}
                onChange={(e) => setFilterEmployeeId(e.target.value)}
                style={{ width: "100%", padding: 8 }}
              >
                <option value="">All</option>
                {employees.map((emp) => (
                  <option key={emp.id} value={emp.id}>
                    {emp.employeeCode} - {emp.fullName}
                  </option>
                ))}
              </select>
            </div>

            <button onClick={applyFilters} style={{ padding: "8px 12px" }}>
              Apply
            </button>
          </div>
        </div>

        {/* Table */}
        <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
          <h3 style={{ marginTop: 0 }}>Attendance Records</h3>

          {loading ? (
            <p>Loading...</p>
          ) : rows.length === 0 ? (
            <p>No attendance records found.</p>
          ) : (
            <div style={{ overflowX: "auto" }}>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr>
                    <Th>Date</Th>
                    <Th>Employee</Th>
                    <Th>Department</Th>
                    <Th>Status</Th>
                    <Th>CheckIn</Th>
                    <Th>CheckOut</Th>
                    <Th>Actions</Th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map((r) => (
                    <tr key={r.id}>
                      <Td>{r.date}</Td>
                      <Td>
                        {r.employeeCode} - {r.employeeName}
                      </Td>
                      <Td>{r.departmentName}</Td>
                      <Td>{r.status}</Td>
                      <Td>{r.checkIn ?? ""}</Td>
                      <Td>{r.checkOut ?? ""}</Td>
                      <Td>
                        <button onClick={() => deleteAttendance(r.id)}>
                          Delete
                        </button>
                      </Td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </>
  );
}

function Th({ children }: { children: React.ReactNode }) {
  return (
    <th
      style={{ textAlign: "left", borderBottom: "1px solid #ddd", padding: 8 }}
    >
      {children}
    </th>
  );
}

function Td({ children }: { children: React.ReactNode }) {
  return (
    <td style={{ borderBottom: "1px solid #eee", padding: 8 }}>{children}</td>
  );
}
