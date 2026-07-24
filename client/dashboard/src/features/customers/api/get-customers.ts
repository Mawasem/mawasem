import { api } from "@/lib/axios";
import type { CustomersQuery, CustomersResponse } from "../types";

export const getCustomers = async (params: CustomersQuery) => {
  const { data } = await api.get<CustomersResponse>("/customers", {
    params
  })

  return data;
}