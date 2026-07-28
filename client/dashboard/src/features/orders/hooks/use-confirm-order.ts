import { useMutation, useQueryClient } from "@tanstack/react-query";
import { confirmOrder } from "../api/confirm-order";
import { orderKeys } from "../order-query-keys";

export function useConfirmOrder() {
  const queryClient = useQueryClient();
  const {
    mutate: confirmOrderMutation,
    mutateAsync: confirmOrderAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: confirmOrder,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all });
      queryClient.invalidateQueries({
        queryKey: orderKeys.detail(variables.orderId),
      });
    },
  });
  return {
    confirmOrder: confirmOrderMutation,
    confirmOrderAsync,
    isLoading,
    error,
  };
}
