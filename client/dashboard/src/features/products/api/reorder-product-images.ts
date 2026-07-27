import { api } from "@/lib/axios";

import type { ProductImage, ReorderProductImagesParams } from "../types";

export async function reorderProductImages({ productId, data }: ReorderProductImagesParams) {
  const response = await api.put<ProductImage[]>(`/products/${productId}/images/order`, data);
  return response.data;
}
