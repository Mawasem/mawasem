import { useQuery } from "@tanstack/react-query"
import { getDeliveryAreas } from "../api/get-delivery-areas"
import { checkoutQueryKeys } from "../query-keys/checkout-query-keys"
export function useDeliveryAreas() {
  const query = useQuery({
    queryKey: checkoutQueryKeys.deliveryAreas,
    queryFn: getDeliveryAreas,
    staleTime: 5 * 60 * 1000,
  })
  return {
    deliveryAreasData: query.data,
    isLoading: query.isPending,
    error: query.error,
  }
}
