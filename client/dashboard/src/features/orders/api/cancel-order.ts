import { api } from "@/lib/axios";
import type { OrderReasonWorkflowParams, OrderWorkflowResponse } from "../types";

export async function cancelOrder({ orderId, data }: OrderReasonWorkflowParams) {
  const response = await api.put<OrderWorkflowResponse>(
    `/orders/${orderId}/cancel`,
    data
  );
  return response.data;
}
