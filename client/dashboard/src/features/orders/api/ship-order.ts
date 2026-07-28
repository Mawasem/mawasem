import { api } from "@/lib/axios";
import type { OrderWorkflowParams, OrderWorkflowResponse } from "../types";

export async function shipOrder({ orderId }: OrderWorkflowParams) {
  const response = await api.put<OrderWorkflowResponse>(
    `/orders/${orderId}/ship`
  );
  return response.data;
}
