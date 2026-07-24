import { api } from "@/lib/axios";
import type { BlockCustomerRequest } from "../types";

export const blockCustomer = async (
  customerId: number,
  body: BlockCustomerRequest
) => {
  const { data } = await api.post(
    `/customers/${customerId}/block`,
    body
  );

  return data;
};