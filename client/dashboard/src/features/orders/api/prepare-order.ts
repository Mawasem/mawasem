import { api } from "@/lib/axios";
import type { OrderWorkflowParams, OrderWorkflowResponse } from "../types";

export async function prepareOrder({ orderId }: OrderWorkflowParams) {
  const response = await api.put<OrderWorkflowResponse>(
    `/orders/${orderId}/prepare`
  );
  return response.data;
}
