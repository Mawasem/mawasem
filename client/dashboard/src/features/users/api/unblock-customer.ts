import { api } from "@/lib/axios";

export const unblockCustomer = async (customerId: number) => {
  const { data } = await api.post(
    `/customers/${customerId}/unblock`
  );

  return data;
};