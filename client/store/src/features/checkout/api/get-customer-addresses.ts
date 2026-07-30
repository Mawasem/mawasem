import { api } from "@/lib/axios"
import type { CustomerAddressListResponse } from "../types/checkout.types"
export async function getCustomerAddresses() {
  const response = await api.get<CustomerAddressListResponse>("/addresses")
  return response.data
}
