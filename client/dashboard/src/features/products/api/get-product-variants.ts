import { api } from "@/lib/axios";

import type { ProductVariant } from "../types";

export async function getProductVariants(productId: number) {
  const response = await api.get<ProductVariant[]>(`/products/${productId}/variants`);
  return response.data;
}
