import { api } from "@/lib/axios"

import type { PublicProductListResponse } from "../types/paginated-response.types"
import type { GetPublicProductsParams } from "../types/product-query.types"

export async function getPublicProducts(params: GetPublicProductsParams) {
  const response = await api.get<PublicProductListResponse>("/products", {
    params,
  })

  return response.data
}
