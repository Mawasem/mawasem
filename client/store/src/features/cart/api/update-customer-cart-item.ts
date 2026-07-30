import { api } from "@/lib/axios"
import type { UpdateCartItemRequest } from "../types/cart.types"

export async function updateCustomerCartItem({
  cartItemId,
  quantity,
}: UpdateCartItemRequest) {
  await api.put(`/carts/customer/items/${cartItemId}`, { quantity })
}
