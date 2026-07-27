import { useMutation, useQueryClient } from "@tanstack/react-query";

import { restoreProduct } from "../api/restore-product";

export function useRestoreProduct() {
  const queryClient = useQueryClient();
  const { mutate: restoreProductMutation, mutateAsync: restoreProductAsync, isPending: isLoading, error } = useMutation({
    mutationFn: restoreProduct,
    onSuccess: (_, productId) => {
      void queryClient.invalidateQueries({ queryKey: ["products"] });
      void queryClient.invalidateQueries({ queryKey: ["product", productId] });
    },
  });
  return { restoreProduct: restoreProductMutation, restoreProductAsync, isLoading, error };
}
