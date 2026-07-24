import { api } from "@/lib/axios"
import type { Customer } from "../types"

export const getCustomer = async (customerId: number) => {
  const data = await api.get<Customer>(`/customer/${customerId}`);

  return data;
}