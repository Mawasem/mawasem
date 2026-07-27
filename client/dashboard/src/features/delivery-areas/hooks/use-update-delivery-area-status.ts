import { useMutation, useQueryClient } from "@tanstack/react-query";

import { updateDeliveryAreaStatus } from "../api/update-delivery-area-status";

export function useUpdateDeliveryAreaStatus() {
  const queryClient = useQueryClient();

  const {
    mutate: updateDeliveryAreaStatusMutation,
    mutateAsync: updateDeliveryAreaStatusAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: updateDeliveryAreaStatus,
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
    updateDeliveryAreaStatus: updateDeliveryAreaStatusMutation,
    updateDeliveryAreaStatusAsync,
    isLoading,
    error,
  };
}
