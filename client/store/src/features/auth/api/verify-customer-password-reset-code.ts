import { api } from "@/lib/axios"
import type {
  VerifyCustomerPasswordResetCodeRequest,
  VerifyCustomerPasswordResetCodeResponse,
} from "../types"
export async function verifyCustomerPasswordResetCode(
  data: VerifyCustomerPasswordResetCodeRequest
) {
  const response = await api.post<VerifyCustomerPasswordResetCodeResponse>(
    "/auth/verify-reset-code",
    data
  )
  return response.data
}
