import { useMutation, useQueryClient } from "@tanstack/react-query";
import { cancelOrder } from "../api/cancel-order";
import { orderKeys } from "../order-query-keys";

export function useCancelOrder() {
  const queryClient = useQueryClient();
  const {
    mutate: cancelOrderMutation,
    mutateAsync: cancelOrderAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: cancelOrder,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all });
      queryClient.invalidateQueries({
        queryKey: orderKeys.detail(variables.orderId),
      });
    },
  });
  return {
    cancelOrder: cancelOrderMutation,
    cancelOrderAsync,
    isLoading,
    error,
  };
}
