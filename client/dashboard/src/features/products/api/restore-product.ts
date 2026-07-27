import { api } from "@/lib/axios";

export async function restoreProduct(productId: number) {
  await api.post(`/products/${productId}/restore`);
}
