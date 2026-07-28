import { useQuery } from "@tanstack/react-query";
import { getAdminOrder } from "../api/get-admin-order";
import { orderKeys } from "../order-query-keys";

export function useAdminOrder(orderId: number, enabled = true) {
  const { data: orderData, isPending: isLoading, error } = useQuery({
    queryKey: orderKeys.detail(orderId),
    queryFn: () => getAdminOrder(orderId),
    enabled: enabled && orderId > 0,
  });
  return { orderData, isLoading, error };
}
