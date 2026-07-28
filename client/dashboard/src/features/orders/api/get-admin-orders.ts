import { api } from "@/lib/axios";
import type { AdminOrdersResponse, GetAdminOrdersParams } from "../types";

export async function getAdminOrders(params: GetAdminOrdersParams) {
  const response = await api.get<AdminOrdersResponse>("/orders", { params });
  return response.data;
}
