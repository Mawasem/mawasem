import { useMutation, useQueryClient } from "@tanstack/react-query"
import { createCustomerAddress } from "../api/create-customer-address"
import { checkoutQueryKeys } from "../query-keys/checkout-query-keys"
export function useCreateCustomerAddress() {
  const queryClient = useQueryClient()
  const mutation = useMutation({
    mutationFn: createCustomerAddress,
    onSuccess: () =>
      void queryClient.invalidateQueries({
        queryKey: checkoutQueryKeys.addresses,
      }),
  })
  return {
    createAddressAsync: mutation.mutateAsync,
    isLoading: mutation.isPending,
    error: mutation.error,
  }
}
