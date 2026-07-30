import { useMutation, useQueryClient } from "@tanstack/react-query"
import { mergeGuestCartAfterAuthentication } from "@/features/cart/services/merge-guest-cart-after-auth"
import { loginCustomer } from "../api/login-customer"
import { useCustomerAuthStore } from "../store/use-customer-auth-store"
export function useLoginCustomer() {
  const queryClient = useQueryClient()
  const setSession = useCustomerAuthStore((state) => state.setSession)
  const {
    mutate: login,
    mutateAsync: loginAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: loginCustomer,
    onSuccess: async (session) => {
      setSession(session)
      await mergeGuestCartAfterAuthentication(queryClient)
    },
  })
  return { login, loginAsync, isLoading, error }
}
