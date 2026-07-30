import { useMutation } from "@tanstack/react-query"
import { forgotCustomerPassword } from "../api/forgot-customer-password"
export function useForgotCustomerPassword() {
  const mutation = useMutation({ mutationFn: forgotCustomerPassword })
  return {
    forgotPassword: mutation.mutate,
    forgotPasswordAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
    isSuccess: mutation.isSuccess,
  }
}
