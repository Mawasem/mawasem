import { api } from "@/lib/axios";

import type { ProductDetails, UpdateProductParams } from "../types";

export async function updateProduct({ productId, data }: UpdateProductParams) {
  const response = await api.put<ProductDetails>(`/products/${productId}`, data);
  return response.data;
}
