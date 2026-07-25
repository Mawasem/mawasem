import { api } from "@/lib/axios";

export async function restoreCollection(
  id: number
) {
  await api.post(
    `/collections/${id}/restore`
  );
}