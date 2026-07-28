import { api } from "@/lib/axios";
import type { AdminOrderDetails } from "../types";

export async function getAdminOrder(orderId: number) {
  const response = await api.get<AdminOrderDetails>(`/orders/${orderId}`);
  return response.data;
}
