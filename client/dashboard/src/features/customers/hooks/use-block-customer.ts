import { useMutation, useQueryClient } from "@tanstack/react-query";

import { blockCustomer } from "../api/block-customer";

export const useBlockCustomer = () => {
  const queryClient = useQueryClient();

  const {
    mutate: blockCustomerMutation,
    mutateAsync: blockCustomerAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: ({
      customerId,
      reason,
    }: {
      customerId: number;
      reason: string;
    }) => blockCustomer(customerId, { reason }),

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["customers"],
      });
    },
  });

  return {
    blockCustomer: blockCustomerMutation,
    blockCustomerAsync,
    isLoading,
    error,
  };
};