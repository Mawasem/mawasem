import { api } from "@/lib/axios";

export async function restoreDeliveryArea(deliveryAreaId: number) {
  await api.post(`/delivery-areas/${deliveryAreaId}/restore`);
}
