import { api } from "@/lib/axios"

export async function clearGuestCart(token: string) {
  await api.delete("/carts/guest/items", {
    headers: { "X-Guest-Cart-Token": token },
  })
}
