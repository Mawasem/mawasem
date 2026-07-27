import { api } from "@/lib/axios";

import type { CreateProductOptionRequest, ProductOption } from "../types";

export async function createProductOption(data: CreateProductOptionRequest) {
  const response = await api.post<ProductOption>("/product-options", data);
  return response.data;
}
