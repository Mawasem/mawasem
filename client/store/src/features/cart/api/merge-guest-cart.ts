import { api } from "@/lib/axios"
import type { CartMergeResponse } from "../types/cart.types"

export async function mergeGuestCart(token: string) {
  const response = await api.post<CartMergeResponse>(
    "/carts/customer/merge-guest",
    { token }
  )
  return response.data
}
