import { useMutation, useQueryClient } from "@tanstack/react-query";

import { deleteProduct } from "../api/delete-product";

export function useDeleteProduct() {
  const queryClient = useQueryClient();
  const { mutate: deleteProductMutation, mutateAsync: deleteProductAsync, isPending: isLoading, error } = useMutation({
    mutationFn: deleteProduct,
    onSuccess: (_, productId) => {
      void queryClient.invalidateQueries({ queryKey: ["products"] });
      void queryClient.invalidateQueries({ queryKey: ["product", productId] });
    },
  });
  return { deleteProduct: deleteProductMutation, deleteProductAsync, isLoading, error };
}
