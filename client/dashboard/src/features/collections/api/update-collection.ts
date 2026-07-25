import { api } from "@/lib/axios";
import type {
  Collection,
  UpdateCollectionParams,
} from "../types";

export async function updateCollection({
  id,
  data,
}: UpdateCollectionParams) {
  const response = await api.put<Collection>(
    `/collections/${id}`,
    data
  );

  return response.data;
}