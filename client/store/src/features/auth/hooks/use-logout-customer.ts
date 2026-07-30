import { useMutation, useQueryClient } from "@tanstack/react-query"
import { logoutCustomer } from "../api/logout-customer"
import { useCustomerAuthStore } from "../store/use-customer-auth-store"
export function useLogoutCustomer() {
  const queryClient = useQueryClient()
  const clearSession = useCustomerAuthStore((state) => state.clearSession)
  const mutation = useMutation({
    mutationFn: logoutCustomer,
    onSettled: () => {
      clearSession()
      queryClient.clear()
    },
  })
  return {
    logout: mutation.mutate,
    logoutAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
  }
}
