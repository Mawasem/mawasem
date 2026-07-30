import { api } from "@/lib/axios"
import type { ForgotCustomerPasswordRequest } from "../types"
export async function forgotCustomerPassword(
  data: ForgotCustomerPasswordRequest
) {
  await api.post("/auth/forgot-password", data)
}
