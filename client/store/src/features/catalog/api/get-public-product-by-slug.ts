import { api } from "@/lib/axios"

import type { PublicProductDetails } from "../types/product-details.types"

export async function getPublicProductBySlug(slug: string) {
  const response = await api.get<PublicProductDetails>(`/products/${slug}`)
  return response.data
}
