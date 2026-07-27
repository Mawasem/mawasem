import { useQuery } from "@tanstack/react-query";

import { getProduct } from "../api/get-product";

export function useProduct(productId: number, enabled = true) {
  const { data: productData, isPending: isLoading, error } = useQuery({
    queryKey: ["product", productId],
    queryFn: () => getProduct(productId),
    enabled: enabled && productId > 0,
  });

  return { productData, isLoading, error };
}
