import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useCustomerAuthStore } from "@/features/auth/store/use-customer-auth-store"
import { addCustomerCartItem } from "../api/add-customer-cart-item"
import { addGuestCartItem } from "../api/add-guest-cart-item"
import { createGuestCart } from "../api/create-guest-cart"
import { cartQueryKeys } from "../query-keys/cart-query-keys"
import {
  getGuestCartToken,
  setGuestCartToken,
} from "../services/guest-cart-token"
import type { AddCartItemRequest } from "../types/cart.types"

export function useAddToCart() {
  const queryClient = useQueryClient()
  const isAuthenticated = useCustomerAuthStore(
    (state) => state.status === "authenticated"
  )

  const mutation = useMutation({
    mutationFn: async (data: AddCartItemRequest) => {
      if (isAuthenticated) {
        await addCustomerCartItem(data)
        return "customer" as const
      }

      let token = getGuestCartToken()
      if (!token) {
        const guestCart = await createGuestCart()
        token = guestCart.token
        setGuestCartToken(token)
      }
      await addGuestCartItem({ ...data, token })
      return token
    },
    onSuccess: (identity) => {
      void queryClient.invalidateQueries({
        queryKey: cartQueryKeys.current(identity),
      })
      void queryClient.invalidateQueries({ queryKey: cartQueryKeys.all })
    },
  })

  return {
    addToCart: mutation.mutate,
    addToCartAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
    isSuccess: mutation.isSuccess,
  }
}
