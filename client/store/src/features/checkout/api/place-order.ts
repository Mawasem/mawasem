import { api } from "@/lib/axios"
import type {
  PlaceOrderRequest,
  PlaceOrderResponse,
} from "../types/checkout.types"
export async function placeOrder(data: PlaceOrderRequest) {
  const response = await api.post<PlaceOrderResponse>(
    "/checkout/place-order",
    data
  )
  return response.data
}
