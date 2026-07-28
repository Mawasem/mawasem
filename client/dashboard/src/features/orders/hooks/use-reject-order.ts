import { useMutation, useQueryClient } from "@tanstack/react-query";
import { rejectOrder } from "../api/reject-order";
import { orderKeys } from "../order-query-keys";

export function useRejectOrder() {
  const queryClient = useQueryClient();
  const {
    mutate: rejectOrderMutation,
    mutateAsync: rejectOrderAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: rejectOrder,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: orderKeys.all });
      queryClient.invalidateQueries({
        queryKey: orderKeys.detail(variables.orderId),
      });
    },
  });
  return {
    rejectOrder: rejectOrderMutation,
    rejectOrderAsync,
    isLoading,
    error,
  };
}
