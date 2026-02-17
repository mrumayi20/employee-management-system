import { useEffect, useState } from "react";
import api from "../api/axios";
import Navbar from "../components/Navbar";
import type { Department } from "../types/models";

type CreateDepartmentForm = {
  name: string;
  description: string;
};

export default function Departments() {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [form, setForm] = useState<CreateDepartmentForm>({
    name: "",
    description: "",
  });

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setError(null);
    setLoading(true);
    try {
      const res = await api.get<Department[]>("/departments");
      setDepartments(res.data);
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to load departments.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function createDepartment(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!form.name.trim()) {
      setError("Department name is required.");
      return;
    }

    try {
      await api.post("/departments", {
        name: form.name,
        description: form.description || null,
      });

      setForm({ name: "", description: "" });
      await load();
    } catch (e: any) {
      setError(e?.response?.data?.error ?? "Failed to create department.");
    }
  }

  return (
    <>
      <Navbar />
      <div style={{ padding: 16, display: "grid", gap: 16 }}>
        <h2>Departments</h2>

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

        {/* Create */}
        <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
          <h3 style={{ marginTop: 0 }}>Add Department</h3>

          <form
            onSubmit={createDepartment}
            style={{
              display: "grid",
              gridTemplateColumns: "repeat(2, minmax(240px, 1fr))",
              gap: 12,
            }}
          >
            <div>
              <label style={{ display: "block", marginBottom: 4 }}>
                Name *
              </label>
              <input
                value={form.name}
                onChange={(e) =>
                  setForm((f) => ({ ...f, name: e.target.value }))
                }
                style={{ width: "100%", padding: 8 }}
                placeholder="Engineering"
              />
            </div>

            <div>
              <label style={{ display: "block", marginBottom: 4 }}>
                Description
              </label>
              <input
                value={form.description}
                onChange={(e) =>
                  setForm((f) => ({ ...f, description: e.target.value }))
                }
                style={{ width: "100%", padding: 8 }}
                placeholder="Builds products"
              />
            </div>

            <div style={{ display: "flex", alignItems: "end" }}>
              <button style={{ padding: "8px 12px" }}>Add</button>
            </div>
          </form>
        </div>

        {/* List */}
        <div style={{ border: "1px solid #ddd", borderRadius: 8, padding: 12 }}>
          <h3 style={{ marginTop: 0 }}>Department List</h3>

          {loading ? (
            <p>Loading...</p>
          ) : departments.length === 0 ? (
            <p>No departments found.</p>
          ) : (
            <div style={{ overflowX: "auto" }}>
              <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                  <tr>
                    <Th>Name</Th>
                    <Th>Description</Th>
                  </tr>
                </thead>
                <tbody>
                  {departments.map((d) => (
                    <tr key={d.id}>
                      <Td>{d.name}</Td>
                      <Td>{d.description ?? ""}</Td>
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
