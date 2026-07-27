import { api } from "@/lib/axios";

import type {
  DeliveryAreasResponse,
  GetDeliveryAreasParams,
} from "../types";

export async function getDeliveryAreas(params: GetDeliveryAreasParams) {
  const response = await api.get<DeliveryAreasResponse>("/delivery-areas", {
    params,
  });

  return response.data;
}
