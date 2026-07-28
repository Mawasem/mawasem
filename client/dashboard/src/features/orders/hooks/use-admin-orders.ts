import { useQuery } from "@tanstack/react-query";
import { getAdminOrders } from "../api/get-admin-orders";
import { orderKeys } from "../order-query-keys";
import type { GetAdminOrdersParams } from "../types";

export function useAdminOrders(params: GetAdminOrdersParams) {
  const { data: ordersData, isPending: isLoading, error } = useQuery({
    queryKey: orderKeys.list(params),
    queryFn: () => getAdminOrders(params),
  });
  return { ordersData, isLoading, error };
}
