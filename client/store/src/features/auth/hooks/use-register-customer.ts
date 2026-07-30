import { useMutation } from "@tanstack/react-query"

import { registerCustomer } from "../api/register-customer"
import { useCustomerAuthStore } from "../store/use-customer-auth-store"

export function useRegisterCustomer() {
  const setSession = useCustomerAuthStore((state) => state.setSession)

  const {
    mutate: registerCustomerMutation,
    mutateAsync: registerCustomerAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: registerCustomer,
    onSuccess: setSession,
  })

  return {
    registerCustomer: registerCustomerMutation,
    registerCustomerAsync,
    isLoading,
    error,
  }
}
