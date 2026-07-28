import { useMutation, useQueryClient } from "@tanstack/react-query";
import { prepareOrder } from "../api/prepare-order";
import { orderKeys } from "../order-query-keys";

export function usePrepareOrder() {
  const queryClient = useQueryClient();
  const {
    mutate: prepareOrderMutation,
    mutateAsync: prepareOrderAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: prepareOrder,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all });
      queryClient.invalidateQueries({
        queryKey: orderKeys.detail(variables.orderId),
      });
    },
  });
  return {
    prepareOrder: prepareOrderMutation,
    prepareOrderAsync,
    isLoading,
    error,
  };
}
