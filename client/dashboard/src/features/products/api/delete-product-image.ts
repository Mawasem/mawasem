import { api } from "@/lib/axios";

import type { ProductImageMutationParams } from "../types";

export async function deleteProductImage({ productId, imageId }: ProductImageMutationParams) {
  await api.delete(`/products/${productId}/images/${imageId}`);
}
