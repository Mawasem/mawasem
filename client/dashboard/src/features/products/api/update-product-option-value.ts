import { api } from "@/lib/axios";

import type { ProductOptionValue, UpdateProductOptionValueParams } from "../types";

export async function updateProductOptionValue({ optionId, valueId, data }: UpdateProductOptionValueParams) {
  const response = await api.put<ProductOptionValue>(`/product-options/${optionId}/values/${valueId}`, data);
  return response.data;
}
