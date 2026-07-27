import { useMutation, useQueryClient } from "@tanstack/react-query";

import { createProduct } from "../api/create-product";

export function useCreateProduct() {
  const queryClient = useQueryClient();
  const { mutate: createProductMutation, mutateAsync: createProductAsync, isPending: isLoading, error } = useMutation({
    mutationFn: createProduct,
    onSuccess: (product) => {
      queryClient.setQueryData(["product", product.id], product);
      void queryClient.invalidateQueries({ queryKey: ["products"] });
    },
  });
  return { createProduct: createProductMutation, createProductAsync, isLoading, error };
}
