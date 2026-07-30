import { useQuery } from "@tanstack/react-query"
import { useCustomerAuthStore } from "@/features/auth/store/use-customer-auth-store"
import { getCustomerCart } from "../api/get-customer-cart"
import { getOrCreateCustomerCart } from "../api/get-or-create-customer-cart"
import { getGuestCart } from "../api/get-guest-cart"
import { cartQueryKeys } from "../query-keys/cart-query-keys"
import { getGuestCartToken } from "../services/guest-cart-token"

export function useCart() {
  const status = useCustomerAuthStore((state) => state.status)
  const isAuthenticated = status === "authenticated"
  const guestToken = getGuestCartToken()
  const identity = isAuthenticated
    ? "customer"
    : (guestToken ?? "no-guest-cart")

  const query = useQuery({
    queryKey: cartQueryKeys.current(identity),
    queryFn: async () => {
      if (!isAuthenticated) return getGuestCart(guestToken!)
      await getOrCreateCustomerCart()
      return getCustomerCart()
    },
    enabled: status !== "checking" && (isAuthenticated || Boolean(guestToken)),
    retry: false,
  })

  return {
    cartData: query.data,
    isLoading: query.isPending && query.fetchStatus !== "idle",
    error: query.error,
    isAuthenticated,
    guestToken,
  }
}
