import { api } from "@/lib/axios";

import type { ProductDetails, ProductPayload } from "../types";

export async function createProduct(data: ProductPayload) {
  const response = await api.post<ProductDetails>("/products", data);
  return response.data;
}
