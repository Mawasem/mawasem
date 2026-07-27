import { useQuery } from "@tanstack/react-query";

import { getProductOptions } from "../api/get-product-options";

export function useProductOptions(enabled = true) {
  const { data: productOptionsData, isPending: isLoading, error } = useQuery({
    queryKey: ["product-options"],
    queryFn: getProductOptions,
    enabled,
  });
  return { productOptionsData, isLoading, error };
}
