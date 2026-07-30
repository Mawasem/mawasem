import { api } from "@/lib/axios"
import type { AddGuestCartItemRequest } from "../types/cart.types"

export async function addGuestCartItem(data: AddGuestCartItemRequest) {
  await api.post("/carts/guest/items", data)
}
