import { api } from "@/lib/axios";

import type { ProductDetails } from "../types";

export async function getProduct(productId: number) {
  const response = await api.get<ProductDetails>(`/products/${productId}`);
  return response.data;
}
