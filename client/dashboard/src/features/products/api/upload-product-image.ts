import { api } from "@/lib/axios";

import type { ProductImage, UploadProductImageParams } from "../types";

export async function uploadProductImage({ productId, image, colorOptionValueId, isPrimary }: UploadProductImageParams) {
  const formData = new FormData();
  formData.append("image", image);
  formData.append("isPrimary", String(isPrimary));
  if (colorOptionValueId !== undefined) {
    formData.append("colorOptionValueId", String(colorOptionValueId));
  }
  const response = await api.post<ProductImage>(`/products/${productId}/images`, formData);
  return response.data;
}
