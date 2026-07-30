import { api } from "@/lib/axios"
import type {
  CheckoutPreview,
  CheckoutPreviewRequest,
} from "../types/checkout.types"
export async function getCheckoutPreview(data: CheckoutPreviewRequest) {
  const response = await api.post<CheckoutPreview>("/checkout/preview", data)
  return response.data
}
