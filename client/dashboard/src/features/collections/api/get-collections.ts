import { api } from "@/lib/axios";
import type { PaginatedResponse } from "@/types/pagination";
import type {
  Collection,
  CollectionQueryParams,
} from "../types";

export async function getCollections({
  search,
  includeDeleted,
  pageNumber,
  pageSize,
}: CollectionQueryParams) {
  const response =
    await api.get<
      PaginatedResponse<Collection>
    >("/collections", {
      params: {
        search,
        includeDeleted,
        pageNumber,
        pageSize,
      },
    });

  return response.data;
}