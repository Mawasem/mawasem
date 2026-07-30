import { api } from "@/lib/axios"

export async function removeCustomerCartItem(cartItemId: number) {
  await api.delete(`/carts/customer/items/${cartItemId}`)
}
