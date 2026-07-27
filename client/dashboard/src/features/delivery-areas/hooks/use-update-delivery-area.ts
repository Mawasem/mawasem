import { useMutation, useQueryClient } from "@tanstack/react-query";

import { updateDeliveryArea } from "../api/update-delivery-area";

export function useUpdateDeliveryArea() {
  const queryClient = useQueryClient();

  const {
    mutate: updateDeliveryAreaMutation,
    mutateAsync: updateDeliveryAreaAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: updateDeliveryArea,
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["delivery-areas"],
      });
      queryClient.invalidateQueries({
        queryKey: ["delivery-area", variables.deliveryAreaId],
      });
    },
  });

  return {
    updateDeliveryArea: updateDeliveryAreaMutation,
    updateDeliveryAreaAsync,
    isLoading,
    error,
  };
}
