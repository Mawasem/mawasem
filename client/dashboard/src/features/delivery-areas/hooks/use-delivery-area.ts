import { useQuery } from "@tanstack/react-query";

import { getDeliveryArea } from "../api/get-delivery-area";

export function useDeliveryArea(
  deliveryAreaId: number,
  enabled = true
) {
  const {
    data: deliveryAreaData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["delivery-area", deliveryAreaId],
    queryFn: () => getDeliveryArea(deliveryAreaId),
    enabled: enabled && deliveryAreaId > 0,
  });

  return {
    deliveryAreaData,
    isLoading,
    error,
  };
}
