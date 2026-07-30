import type { QueryClient } from "@tanstack/react-query"
import { mergeGuestCart } from "../api/merge-guest-cart"
import { cartQueryKeys } from "../query-keys/cart-query-keys"
import { clearGuestCartToken, getGuestCartToken } from "./guest-cart-token"

export async function mergeGuestCartAfterAuthentication(
  queryClient: QueryClient
) {
  const token = getGuestCartToken()
  if (!token) return
  try {
    await mergeGuestCart(token)
    clearGuestCartToken()
    await queryClient.invalidateQueries({ queryKey: cartQueryKeys.all })
  } catch {
    // Keep the token so the customer can retry the merge on a later login.
  }
}
