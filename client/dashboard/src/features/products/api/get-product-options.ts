import { api } from "@/lib/axios";

import type { ProductOption } from "../types";

export async function getProductOptions() {
  const response = await api.get<ProductOption[]>("/product-options");
  return response.data;
}
