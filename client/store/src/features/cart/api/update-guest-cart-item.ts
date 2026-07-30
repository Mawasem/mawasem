import { api } from "@/lib/axios"
import type { UpdateCartItemRequest } from "../types/cart.types"

export async function updateGuestCartItem(
  token: string,
  data: UpdateCartItemRequest
) {
  await api.put(`/carts/guest/items/${data.cartItemId}`, {
    token,
    quantity: data.quantity,
  })
}
