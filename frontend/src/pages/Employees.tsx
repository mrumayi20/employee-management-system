import { useEffect, useMemo, useState } from "react";
import api from "../api/axios";
import Navbar from "../components/Navbar";
import type { Department, Employee } from "../types/models";

type CreateEmployeeForm = {
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  dateOfJoining: string; // yyyy-mm-dd
  departmentId: string;
};

export default function Employees() {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [form, setForm] = useState<CreateEmployeeForm>({
    employeeCode: "",
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    dateOfJoining: "",
    departmentId: "",
  });

  const canSubmit = useMemo(() => {
    return (
      form.employeeCode.trim() &&
      form.firstName.trim() &&
      form.lastName.trim() &&
      form.email.trim() &&
      form.phone.trim() &&
      form.dateOfJoining.trim() &&
      form.departmentId.trim()
    );
  }, [form]);

  async function load() {
    setError(null);
    setLoading(true);
    try {
      const [empRes, deptRes] = await Promise.all([
        api.get<Employee[]>("/employees"),
        api.get<Department[]>("/departments"),
      ]);

      setEmployees(empRes.data);
      setDepartments(deptRes.data);

      // default dropdown to first department
      if (!form.departmentId && deptRes.data.length > 0) {
        setForm((f) => ({ ...f, departmentId: deptRes.data[0].id }));
      }
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to load employees.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function createEmployee(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!canSubmit) {
      setError("Please fill all fields.");
      return;
    }

    try {
      await api.post("/employees", {
        employeeCode: form.employeeCode,
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        phone: form.phone,
        dateOfJoining: `${form.dateOfJoining}T00:00:00`,
        departmentId: form.departmentId,
      });

      // reset form (keep department)
      setForm((f) => ({
        employeeCode: "",
        firstName: "",
        lastName: "",
        email: "",
        phone: "",
        dateOfJoining: "",
        departmentId: f.departmentId,
      }));

      await load();
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to create employee.");
    }
  }

  async function deleteEmployee(id: string) {
    const ok = confirm("Delete this employee?");
    if (!ok) return;

    setError(null);
    try {
      await api.delete(`/employees/${id}`);
      setEmployees((prev) => prev.filter((x) => x.id !== id));
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to delete employee.");
    }
  }

  return (
    <>
      <Navbar />
      <div style={{ padding: 16, display: "grid", gap: 16 }}>
        <h2>Employees</h2>

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

        {/* Create Employee */}
        <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
          <h3 style={{ marginTop: 0 }}>Add Employee</h3>

          {departments.length === 0 ? (
            <p>
              No departments found. Create a department first in the Departments
              page.
            </p>
          ) : (
            <form
              onSubmit={createEmployee}
              style={{
                display: "grid",
                gridTemplateColumns: "repeat(2, minmax(240px, 1fr))",
                gap: 12,
              }}
            >
              <Field
                label="Employee Code"
                value={form.employeeCode}
                onChange={(v) => setForm((f) => ({ ...f, employeeCode: v }))}
              />
              <Field
                label="Email"
                value={form.email}
                onChange={(v) => setForm((f) => ({ ...f, email: v }))}
              />
              <Field
                label="First Name"
                value={form.firstName}
                onChange={(v) => setForm((f) => ({ ...f, firstName: v }))}
              />
              <Field
                label="Last Name"
                value={form.lastName}
                onChange={(v) => setForm((f) => ({ ...f, lastName: v }))}
              />
              <Field
                label="Phone"
                value={form.phone}
                onChange={(v) => setForm((f) => ({ ...f, phone: v }))}
              />
              <div>
                <label style={{ display: "block", marginBottom: 4 }}>
                  Date Of Joining
                </label>
                <input
                  type="date"
                  value={form.dateOfJoining}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, dateOfJoining: e.target.value }))
                  }
                  style={{ width: "100%", padding: 8 }}
                />
              </div>

              <div>
                <label style={{ display: "block", marginBottom: 4 }}>
                  Department
                </label>
                <select
                  value={form.departmentId}
                  onChange={(e) =>
                    setForm((f) => ({ ...f, departmentId: e.target.value }))
                  }
                  style={{ width: "100%", padding: 8 }}
                >
                  {departments.map((d) => (
                    <option key={d.id} value={d.id}>
                      {d.name}
                    </option>
                  ))}
                </select>
              </div>

              <div style={{ display: "flex", alignItems: "end" }}>
                <button disabled={!canSubmit} style={{ padding: "8px 12px" }}>
                  Add
                </button>
              </div>
            </form>
          )}
        </div>

        {/* Employees Table */}
        <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
          <h3 style={{ marginTop: 0 }}>Employee List</h3>

          {loading ? (
            <p>Loading...</p>
          ) : employees.length === 0 ? (
            <p>No employees found.</p>
          ) : (
            <div style={{ overflowX: "auto" }}>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr>
                    <Th>Code</Th>
                    <Th>Name</Th>
                    <Th>Email</Th>
                    <Th>Phone</Th>
                    <Th>Department</Th>
                    <Th>Actions</Th>
                  </tr>
                </thead>
                <tbody>
                  {employees.map((e) => (
                    <tr key={e.id}>
                      <Td>{e.employeeCode}</Td>
                      <Td>{e.fullName}</Td>
                      <Td>{e.email}</Td>
                      <Td>{e.phone}</Td>
                      <Td>{e.departmentName}</Td>
                      <Td>
                        <button onClick={() => deleteEmployee(e.id)}>
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

function Field({
  label,
  value,
  onChange,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
}) {
  return (
    <div>
      <label style={{ display: "block", marginBottom: 4 }}>{label}</label>
      <input
        value={value}
        onChange={(e) => onChange(e.target.value)}
        style={{ width: "100%", padding: 8 }}
      />
    </div>
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
