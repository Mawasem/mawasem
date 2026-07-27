import { api } from "@/lib/axios";

import type { ProductImage, ProductImageMutationParams } from "../types";

export async function setPrimaryProductImage({ productId, imageId }: ProductImageMutationParams) {
  const response = await api.put<ProductImage>(`/products/${productId}/images/${imageId}/primary`);
  return response.data;
}
