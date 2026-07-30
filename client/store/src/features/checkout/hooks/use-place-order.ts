import { useMutation, useQueryClient } from "@tanstack/react-query"
import { placeOrder } from "../api/place-order"
import { cartQueryKeys } from "@/features/cart/query-keys/cart-query-keys"
export function usePlaceOrder() {
  const queryClient = useQueryClient()
  const mutation = useMutation({
    mutationFn: placeOrder,
    onSuccess: () =>
      void queryClient.invalidateQueries({ queryKey: cartQueryKeys.all }),
  })
  return {
    placeOrderAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
    reset: mutation.reset,
  }
}
