import { api } from "@/lib/axios";

import type {
  DeliveryArea,
  UpdateDeliveryAreaStatusParams,
} from "../types";

export async function updateDeliveryAreaStatus({
  deliveryAreaId,
  data,
}: UpdateDeliveryAreaStatusParams) {
  const response = await api.put<DeliveryArea>(
    `/delivery-areas/${deliveryAreaId}/status`,
    data
  );

  return response.data;
}
