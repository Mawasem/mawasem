import { api } from "@/lib/axios"
import type { DeliveryAreaListResponse } from "../types/checkout.types"
export async function getDeliveryAreas() {
  const response = await api.get<DeliveryAreaListResponse>("/delivery-areas")
  return response.data
}
