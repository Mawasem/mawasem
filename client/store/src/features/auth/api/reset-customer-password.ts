import { api } from "@/lib/axios"
import type { ResetCustomerPasswordRequest } from "../types"
export async function resetCustomerPassword(
  data: ResetCustomerPasswordRequest
) {
  await api.post("/auth/reset-password", data)
}
