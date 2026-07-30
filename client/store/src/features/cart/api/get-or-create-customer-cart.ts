import { api } from "@/lib/axios"

export async function getOrCreateCustomerCart() {
  await api.post("/carts/customer")
}
