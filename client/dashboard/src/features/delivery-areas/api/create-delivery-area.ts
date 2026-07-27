import { api } from "@/lib/axios";

import type {
  CreateDeliveryAreaRequest,
  DeliveryArea,
} from "../types";

export async function createDeliveryArea(data: CreateDeliveryAreaRequest) {
  const response = await api.post<DeliveryArea>("/delivery-areas", data);

  return response.data;
}
