import { api } from "@/lib/axios";
import type {
  Collection,
  CollectionPayload,
} from "../types";

export async function createCollection(
  data: CollectionPayload
) {
  const response = await api.post<Collection>(
    "/collections",
    data
  );

  return response.data;
}