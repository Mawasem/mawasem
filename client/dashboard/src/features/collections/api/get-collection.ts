import { api } from "@/lib/axios";
import type { Collection } from "../types";

export async function getCollection(
  id: number
) {
  const response = await api.get<Collection>(
    `/collections/${id}`
  );

  return response.data;
}