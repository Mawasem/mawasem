import { api } from "@/lib/axios";

import type { CreateProductVariantParams, ProductVariant } from "../types";

export async function createProductVariant({ productId, data }: CreateProductVariantParams) {
  const response = await api.post<ProductVariant>(`/products/${productId}/variants`, data);
  return response.data;
}
