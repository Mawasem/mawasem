import { api } from "@/lib/axios"
import type { CartDetails } from "../types/cart.types"

export async function getGuestCart(token: string) {
  const response = await api.get<CartDetails>("/carts/guest", {
    headers: { "X-Guest-Cart-Token": token },
  })
  return response.data
}
