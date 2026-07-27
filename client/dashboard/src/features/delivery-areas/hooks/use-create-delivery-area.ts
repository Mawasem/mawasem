import { useMutation, useQueryClient } from "@tanstack/react-query";

import { createDeliveryArea } from "../api/create-delivery-area";

export function useCreateDeliveryArea() {
  const queryClient = useQueryClient();

  const {
    mutate: createDeliveryAreaMutation,
    mutateAsync: createDeliveryAreaAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: createDeliveryArea,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["delivery-areas"],
      });
    },
  });

  return {
    createDeliveryArea: createDeliveryAreaMutation,
    createDeliveryAreaAsync,
    isLoading,
    error,
  };
}
