import { api } from "@/lib/axios"

export async function clearCustomerCart() {
  await api.delete("/carts/customer/items")
}
