import { api } from "@/lib/axios"
export async function logoutCustomer() {
  await api.post("/auth/logout")
}
