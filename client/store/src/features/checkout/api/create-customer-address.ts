import { api } from "@/lib/axios"
import type {
  CreateCustomerAddressRequest,
  CustomerAddress,
} from "../types/checkout.types"
export async function createCustomerAddress(
  data: CreateCustomerAddressRequest
) {
  const response = await api.post<CustomerAddress>("/addresses", data)
  return response.data
}
