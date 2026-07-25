import { api } from "@/lib/axios";

export async function deleteCollection(
  id: number
) {
  await api.delete(
    `/collections/${id}`
  );
}