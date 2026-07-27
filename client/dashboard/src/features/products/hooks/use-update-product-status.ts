import { useMutation, useQueryClient } from "@tanstack/react-query";

import { updateProductStatus } from "../api/update-product-status";

export function useUpdateProductStatus() {
  const queryClient = useQueryClient();
  const { mutate: updateProductStatusMutation, mutateAsync: updateProductStatusAsync, isPending: isLoading, error } = useMutation({
    mutationFn: updateProductStatus,
    onSuccess: (product, variables) => {
      queryClient.setQueryData(["product", variables.productId], product);
      void queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });
  return { updateProductStatus: updateProductStatusMutation, updateProductStatusAsync, isLoading, error };
}
