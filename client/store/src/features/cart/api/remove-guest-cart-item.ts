import { api } from "@/lib/axios"

export async function removeGuestCartItem(token: string, cartItemId: number) {
  await api.delete(`/carts/guest/items/${cartItemId}`, {
    headers: { "X-Guest-Cart-Token": token },
  })
}
