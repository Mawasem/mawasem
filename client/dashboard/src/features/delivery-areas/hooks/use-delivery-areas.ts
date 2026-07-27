import { useQuery } from "@tanstack/react-query";

import { getDeliveryAreas } from "../api/get-delivery-areas";
import type { GetDeliveryAreasParams } from "../types";

export function useDeliveryAreas(params: GetDeliveryAreasParams) {
  const {
    data: deliveryAreasData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["delivery-areas", params],
    queryFn: () => getDeliveryAreas(params),
  });

  return {
    deliveryAreasData,
    isLoading,
    error,
  };
}
