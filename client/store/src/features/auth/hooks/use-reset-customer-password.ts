import { useMutation } from "@tanstack/react-query"
import { resetCustomerPassword } from "../api/reset-customer-password"
export function useResetCustomerPassword() {
  const mutation = useMutation({ mutationFn: resetCustomerPassword })
  return {
    resetPassword: mutation.mutate,
    resetPasswordAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
    isSuccess: mutation.isSuccess,
  }
}
