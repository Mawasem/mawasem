import { useMutation, useQueryClient } from "@tanstack/react-query";

import { updateProduct } from "../api/update-product";

export function useUpdateProduct() {
  const queryClient = useQueryClient();
  const { mutate: updateProductMutation, mutateAsync: updateProductAsync, isPending: isLoading, error } = useMutation({
    mutationFn: updateProduct,
    onSuccess: (product, variables) => {
      queryClient.setQueryData(["product", variables.productId], product);
      void queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });
  return { updateProduct: updateProductMutation, updateProductAsync, isLoading, error };
}
