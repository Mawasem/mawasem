import { useMutation, useQueryClient } from "@tanstack/react-query"
import { clearCustomerCart } from "../api/clear-customer-cart"
import { clearGuestCart } from "../api/clear-guest-cart"
import { cartQueryKeys } from "../query-keys/cart-query-keys"
import { useCart } from "./use-cart"

export function useClearCart() {
  const queryClient = useQueryClient()
  const { isAuthenticated, guestToken } = useCart()
  const mutation = useMutation({
    mutationFn: () =>
      isAuthenticated ? clearCustomerCart() : clearGuestCart(guestToken!),
    onSuccess: () =>
      void queryClient.invalidateQueries({ queryKey: cartQueryKeys.all }),
  })
  return {
    clearCartAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
  }
}
