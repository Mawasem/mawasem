import { api } from "@/lib/axios";

import type { ProductVariant, UpdateProductVariantStockParams } from "../types";

export async function updateProductVariantStock({ productId, variantId, data }: UpdateProductVariantStockParams) {
  const response = await api.put<ProductVariant>(`/products/${productId}/variants/${variantId}/stock`, data);
  return response.data;
}
