import { useMutation, useQueryClient } from "@tanstack/react-query";
import { shipOrder } from "../api/ship-order";
import { orderKeys } from "../order-query-keys";

export function useShipOrder() {
  const queryClient = useQueryClient();
  const {
    mutate: shipOrderMutation,
    mutateAsync: shipOrderAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: shipOrder,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all });
      queryClient.invalidateQueries({
        queryKey: orderKeys.detail(variables.orderId),
      });
    },
  });
  return {
    shipOrder: shipOrderMutation,
    shipOrderAsync,
    isLoading,
    error,
  };
}
