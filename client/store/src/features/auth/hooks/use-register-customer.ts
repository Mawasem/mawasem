import { useMutation, useQueryClient } from "@tanstack/react-query"
import { mergeGuestCartAfterAuthentication } from "@/features/cart/services/merge-guest-cart-after-auth"

import { registerCustomer } from "../api/register-customer"
import { useCustomerAuthStore } from "../store/use-customer-auth-store"

export function useRegisterCustomer() {
  const queryClient = useQueryClient()
  const setSession = useCustomerAuthStore((state) => state.setSession)

  const {
    mutate: registerCustomerMutation,
    mutateAsync: registerCustomerAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: registerCustomer,
    onSuccess: async (session) => {
      setSession(session)
      await mergeGuestCartAfterAuthentication(queryClient)
    },
  })

  return {
    registerCustomer: registerCustomerMutation,
    registerCustomerAsync,
    isLoading,
    error,
  }
}
