import { useMutation, useQueryClient } from "@tanstack/react-query";

import { unblockCustomer } from "../api/unblock-customer";

export const useUnblockCustomer = () => {
  const queryClient = useQueryClient();

  const {
    mutate: unblockCustomerMutation,
    mutateAsync: unblockCustomerAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: (customerId: number) => unblockCustomer(customerId),

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["customers"],
      });
    },
  });

  return {
    unblockCustomer: unblockCustomerMutation,
    unblockCustomerAsync,
    isLoading,
    error,
  };
};