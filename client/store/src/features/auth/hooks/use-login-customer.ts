import { useMutation } from "@tanstack/react-query"
import { loginCustomer } from "../api/login-customer"
import { useCustomerAuthStore } from "../store/use-customer-auth-store"
export function useLoginCustomer() {
  const setSession = useCustomerAuthStore((state) => state.setSession)
  const {
    mutate: login,
    mutateAsync: loginAsync,
    isPending: isLoading,
    error,
  } = useMutation({ mutationFn: loginCustomer, onSuccess: setSession })
  return { login, loginAsync, isLoading, error }
}
