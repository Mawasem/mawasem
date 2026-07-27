import { api } from "@/lib/axios";

import type { GetProductsParams, ProductsResponse } from "../types";

export async function getProducts(params: GetProductsParams) {
  const response = await api.get<ProductsResponse>("/products", { params });
  return response.data;
}
