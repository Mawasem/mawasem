import { useMutation, useQueryClient } from "@tanstack/react-query";

import { restoreDeliveryArea } from "../api/restore-delivery-area";

export function useRestoreDeliveryArea() {
  const queryClient = useQueryClient();

  const {
    mutate: restoreDeliveryAreaMutation,
    mutateAsync: restoreDeliveryAreaAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: restoreDeliveryArea,
    onSuccess: (_, deliveryAreaId) => {
      queryClient.invalidateQueries({
        queryKey: ["delivery-areas"],
      });
      queryClient.invalidateQueries({
        queryKey: ["delivery-area", deliveryAreaId],
      });
    },
  });

  return {
    restoreDeliveryArea: restoreDeliveryAreaMutation,
    restoreDeliveryAreaAsync,
    isLoading,
    error,
  };
}
