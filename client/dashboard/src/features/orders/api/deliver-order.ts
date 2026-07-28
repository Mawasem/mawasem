import { api } from "@/lib/axios";
import type { OrderWorkflowParams, OrderWorkflowResponse } from "../types";

export async function deliverOrder({ orderId }: OrderWorkflowParams) {
  const response = await api.put<OrderWorkflowResponse>(
    `/orders/${orderId}/deliver`
  );
  return response.data;
}
