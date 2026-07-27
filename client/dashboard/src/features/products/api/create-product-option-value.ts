import { api } from "@/lib/axios";

import type { CreateProductOptionValueParams, ProductOptionValue } from "../types";

export async function createProductOptionValue({ optionId, data }: CreateProductOptionValueParams) {
  const response = await api.post<ProductOptionValue>(`/product-options/${optionId}/values`, data);
  return response.data;
}
