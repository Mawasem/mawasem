import { api } from "@/lib/axios";

export async function deleteDeliveryArea(deliveryAreaId: number) {
  await api.delete(`/delivery-areas/${deliveryAreaId}`);
}
