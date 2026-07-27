import { api } from "@/lib/axios";

import type { ProductVariant, UpdateProductVariantAvailabilityParams } from "../types";

export async function updateProductVariantAvailability({ productId, variantId, data }: UpdateProductVariantAvailabilityParams) {
  const response = await api.put<ProductVariant>(`/products/${productId}/variants/${variantId}/availability`, data);
  return response.data;
}
