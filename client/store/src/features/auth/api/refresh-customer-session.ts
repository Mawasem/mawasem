import { refreshApi } from "@/lib/axios"
import type { CustomerAuthenticationResponse } from "../types"

export async function refreshCustomerSession() {
  const response =
    await refreshApi.post<CustomerAuthenticationResponse>("/auth/refresh")
  return response.data
}
