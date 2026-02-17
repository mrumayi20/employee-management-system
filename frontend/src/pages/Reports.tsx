import { useState } from "react";
import Navbar from "../components/Navbar";
import { downloadFile } from "../api/download";

export default function Reports() {
  const [error, setError] = useState<string | null>(null);

  // Attendance report range
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");

  // Salary report month
  const [year, setYear] = useState(new Date().getFullYear());
  const [month, setMonth] = useState(new Date().getMonth() + 1);

  async function run(action: () => Promise<void>) {
    setError(null);
    try {
      await action();
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to download report.");
    }
  }

  return (
    <>
      <Navbar />
      <div style={{ padding: 16, display: "grid", gap: 16 }}>
        <h2>Reports</h2>

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

        {/* Employee Directory */}
        <Card title="Employee Directory">
          <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
            <button
              onClick={() =>
                run(() =>
                  downloadFile(
                    "/reports/employees/pdf",
                    `employee-directory-${stamp()}.pdf`,
                  ),
                )
              }
            >
              Download PDF
            </button>

            <button
              onClick={() =>
                run(() =>
                  downloadFile(
                    "/reports/employees/excel",
                    `employee-directory-${stamp()}.xlsx`,
                  ),
                )
              }
            >
              Download Excel
            </button>
          </div>
        </Card>

        {/* Departments */}
        <Card title="Departments">
          <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
            <button
              onClick={() =>
                run(() =>
                  downloadFile(
                    "/reports/departments/pdf",
                    `departments-${stamp()}.pdf`,
                  ),
                )
              }
            >
              Download PDF
            </button>

            <button
              onClick={() =>
                run(() =>
                  downloadFile(
                    "/reports/departments/excel",
                    `departments-${stamp()}.xlsx`,
                  ),
                )
              }
            >
              Download Excel
            </button>
          </div>
        </Card>

        {/* Attendance */}
        <Card title="Attendance (choose date range)">
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(3, minmax(220px, 1fr))",
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

            <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
              <button
                onClick={() => {
                  if (!from || !to)
                    return setError("Select From and To dates first.");
                  run(() =>
                    downloadFile(
                      "/reports/attendance/pdf",
                      `attendance-${from}-${to}.pdf`,
                      { from, to },
                    ),
                  );
                }}
              >
                PDF
              </button>

              <button
                onClick={() => {
                  if (!from || !to)
                    return setError("Select From and To dates first.");
                  run(() =>
                    downloadFile(
                      "/reports/attendance/excel",
                      `attendance-${from}-${to}.xlsx`,
                      { from, to },
                    ),
                  );
                }}
              >
                Excel
              </button>
            </div>
          </div>
        </Card>

        {/* Salary */}
        <Card title="Salary (choose year/month)">
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(3, minmax(220px, 1fr))",
              gap: 12,
              alignItems: "end",
            }}
          >
            <div>
              <label style={{ display: "block", marginBottom: 4 }}>Year</label>
              <input
                type="number"
                value={year}
                onChange={(e) => setYear(Number(e.target.value))}
                style={{ width: "100%", padding: 8 }}
                min={2000}
                max={2100}
              />
            </div>

            <div>
              <label style={{ display: "block", marginBottom: 4 }}>Month</label>
              <input
                type="number"
                value={month}
                onChange={(e) => setMonth(Number(e.target.value))}
                style={{ width: "100%", padding: 8 }}
                min={1}
                max={12}
              />
            </div>

            <div style={{ display: "flex", gap: 12, flexWrap: "wrap" }}>
              <button
                onClick={() =>
                  run(() =>
                    downloadFile(
                      "/reports/salary/pdf",
                      `salary-${year}-${String(month).padStart(2, "0")}.pdf`,
                      { year, month },
                    ),
                  )
                }
              >
                PDF
              </button>

              <button
                onClick={() =>
                  run(() =>
                    downloadFile(
                      "/reports/salary/excel",
                      `salary-${year}-${String(month).padStart(2, "0")}.xlsx`,
                      { year, month },
                    ),
                  )
                }
              >
                Excel
              </button>
            </div>
          </div>
        </Card>
      </div>
    </>
  );
}

function Card({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) {
  return (
    <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
      <h3 style={{ marginTop: 0 }}>{title}</h3>
      {children}
    </div>
  );
}

function stamp() {
  const d = new Date();
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const dd = String(d.getDate()).padStart(2, "0");
  const hh = String(d.getHours()).padStart(2, "0");
  const mi = String(d.getMinutes()).padStart(2, "0");
  return `${yyyy}${mm}${dd}${hh}${mi}`;
}
