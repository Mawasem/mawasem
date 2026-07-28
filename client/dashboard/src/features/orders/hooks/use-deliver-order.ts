import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deliverOrder } from "../api/deliver-order";
import { orderKeys } from "../order-query-keys";

export function useDeliverOrder() {
  const queryClient = useQueryClient();
  const {
    mutate: deliverOrderMutation,
    mutateAsync: deliverOrderAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: deliverOrder,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all });
      queryClient.invalidateQueries({
        queryKey: orderKeys.detail(variables.orderId),
      });
    },
  });
  return {
    deliverOrder: deliverOrderMutation,
    deliverOrderAsync,
    isLoading,
    error,
  };
}
