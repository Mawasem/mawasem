import { api } from "@/lib/axios";
import type { OrderReasonWorkflowParams, OrderWorkflowResponse } from "../types";

export async function rejectOrder({ orderId, data }: OrderReasonWorkflowParams) {
  const response = await api.put<OrderWorkflowResponse>(
    `/orders/${orderId}/reject`,
    data
  );
  return response.data;
}
