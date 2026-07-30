import { api } from "@/lib/axios"
import type { GuestCartCreationResponse } from "../types/cart.types"

export async function createGuestCart() {
  const response = await api.post<GuestCartCreationResponse>("/carts/guest")
  return response.data
}
