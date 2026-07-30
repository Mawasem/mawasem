import { useMutation } from "@tanstack/react-query"
import { verifyCustomerPasswordResetCode } from "../api/verify-customer-password-reset-code"
export function useVerifyCustomerPasswordResetCode() {
  const mutation = useMutation({ mutationFn: verifyCustomerPasswordResetCode })
  return {
    verifyCode: mutation.mutate,
    verifyCodeAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
  }
}
