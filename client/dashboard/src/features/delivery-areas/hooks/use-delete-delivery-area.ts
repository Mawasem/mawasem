import { useMutation, useQueryClient } from "@tanstack/react-query";

import { deleteDeliveryArea } from "../api/delete-delivery-area";

export function useDeleteDeliveryArea() {
  const queryClient = useQueryClient();

  const {
    mutate: deleteDeliveryAreaMutation,
    mutateAsync: deleteDeliveryAreaAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: deleteDeliveryArea,
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
    deleteDeliveryArea: deleteDeliveryAreaMutation,
    deleteDeliveryAreaAsync,
    isLoading,
    error,
  };
}
