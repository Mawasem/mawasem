import { api } from "@/lib/axios"

import type {
  CustomerAuthenticationResponse,
  RegisterCustomerRequest,
} from "../types"

export async function registerCustomer(data: RegisterCustomerRequest) {
  const response = await api.post<CustomerAuthenticationResponse>(
    "/auth/register",
    data
  )

  return response.data
}
