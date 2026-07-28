import { api } from "@/lib/axios";
import type { OrderWorkflowParams, OrderWorkflowResponse } from "../types";

export async function confirmOrder({ orderId }: OrderWorkflowParams) {
  const response = await api.put<OrderWorkflowResponse>(
    `/orders/${orderId}/confirm`
  );
  return response.data;
}
