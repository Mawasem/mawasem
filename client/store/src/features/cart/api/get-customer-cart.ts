import { api } from "@/lib/axios"
import type { CartDetails } from "../types/cart.types"

export async function getCustomerCart() {
  const response = await api.get<CartDetails>("/carts/customer")
  return response.data
}
