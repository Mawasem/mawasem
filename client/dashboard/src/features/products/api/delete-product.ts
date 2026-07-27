import { api } from "@/lib/axios";

export async function deleteProduct(productId: number) {
  await api.delete(`/products/${productId}`);
}
