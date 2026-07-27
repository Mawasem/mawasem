import { api } from "@/lib/axios";

import type {
  DeliveryArea,
  UpdateDeliveryAreaParams,
} from "../types";

export async function updateDeliveryArea({
  deliveryAreaId,
  data,
}: UpdateDeliveryAreaParams) {
  const response = await api.put<DeliveryArea>(
    `/delivery-areas/${deliveryAreaId}`,
    data
  );

  return response.data;
}
