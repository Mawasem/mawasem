import { useMutation, useQueryClient } from "@tanstack/react-query"
import { removeCustomerCartItem } from "../api/remove-customer-cart-item"
import { removeGuestCartItem } from "../api/remove-guest-cart-item"
import { cartQueryKeys } from "../query-keys/cart-query-keys"
import { useCart } from "./use-cart"

export function useRemoveCartItem() {
  const queryClient = useQueryClient()
  const { isAuthenticated, guestToken } = useCart()
  const mutation = useMutation({
    mutationFn: (cartItemId: number) =>
      isAuthenticated
        ? removeCustomerCartItem(cartItemId)
        : removeGuestCartItem(guestToken!, cartItemId),
    onSuccess: () =>
      void queryClient.invalidateQueries({ queryKey: cartQueryKeys.all }),
  })
  return {
    removeCartItemAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
  }
}
