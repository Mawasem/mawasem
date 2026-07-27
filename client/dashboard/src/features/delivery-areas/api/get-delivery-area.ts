import { api } from "@/lib/axios";

import type { DeliveryArea } from "../types";

export async function getDeliveryArea(deliveryAreaId: number) {
  const response = await api.get<DeliveryArea>(
    `/delivery-areas/${deliveryAreaId}`
  );

  return response.data;
}
