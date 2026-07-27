import { api } from "@/lib/axios";

import type { ProductImage } from "../types";

export async function getProductImages(productId: number) {
  const response = await api.get<ProductImage[]>(`/products/${productId}/images`);
  return response.data;
}
