import { api } from "@/lib/axios"
import type { CustomerDetails } from "../types"

export const getCustomer = async (customerId: number) => {
  const { data } = await api.get<CustomerDetails>(
    `/customers/${customerId}`
  );

  return data;
}