import { api } from "@/lib/axios"
import type { AddCartItemRequest } from "../types/cart.types"

export async function addCustomerCartItem(data: AddCartItemRequest) {
  await api.post("/carts/customer/items", data)
}
