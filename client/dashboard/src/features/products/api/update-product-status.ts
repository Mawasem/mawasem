import { api } from "@/lib/axios";

import type { ProductDetails, UpdateProductStatusParams } from "../types";

export async function updateProductStatus({ productId, data }: UpdateProductStatusParams) {
  const response = await api.put<ProductDetails>(`/products/${productId}/status`, data);
  return response.data;
}
