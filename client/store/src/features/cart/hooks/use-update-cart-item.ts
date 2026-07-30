import { useMutation, useQueryClient } from "@tanstack/react-query"
import { updateCustomerCartItem } from "../api/update-customer-cart-item"
import { updateGuestCartItem } from "../api/update-guest-cart-item"
import { cartQueryKeys } from "../query-keys/cart-query-keys"
import { useCart } from "./use-cart"
import type { UpdateCartItemRequest } from "../types/cart.types"

export function useUpdateCartItem() {
  const queryClient = useQueryClient()
  const { isAuthenticated, guestToken } = useCart()
  const mutation = useMutation({
    mutationFn: (data: UpdateCartItemRequest) =>
      isAuthenticated
        ? updateCustomerCartItem(data)
        : updateGuestCartItem(guestToken!, data),
    onSuccess: () =>
      void queryClient.invalidateQueries({ queryKey: cartQueryKeys.all }),
  })
  return {
    updateCartItemAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
  }
}
