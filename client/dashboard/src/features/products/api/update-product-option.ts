import { api } from "@/lib/axios";

import type { ProductOption, UpdateProductOptionParams } from "../types";

export async function updateProductOption({ optionId, data }: UpdateProductOptionParams) {
  const response = await api.put<ProductOption>(`/product-options/${optionId}`, data);
  return response.data;
}
