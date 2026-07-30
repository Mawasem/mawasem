import { api } from "@/lib/axios"
import type {
  CustomerAuthenticationResponse,
  LoginCustomerRequest,
} from "../types"

export async function loginCustomer(data: LoginCustomerRequest) {
  const response = await api.post<CustomerAuthenticationResponse>(
    "/auth/login",
    data
  )
  return response.data
}
