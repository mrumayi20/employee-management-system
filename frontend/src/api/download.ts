import api from "./axios";

export async function downloadFile(
  url: string,
  filename: string,
  params?: Record<string, any>
) {
  const res = await api.get(url, {
    params,
    responseType: "blob",
  });

  const blob = new Blob([res.data]);
  const link = document.createElement("a");
  link.href = window.URL.createObjectURL(blob);
  link.download = filename;
  link.click();

  window.URL.revokeObjectURL(link.href);
}