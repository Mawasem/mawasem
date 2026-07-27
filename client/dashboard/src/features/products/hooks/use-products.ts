import { useQuery } from "@tanstack/react-query";

import { getProducts } from "../api/get-products";
import type { GetProductsParams } from "../types";

export function useProducts(params: GetProductsParams) {
  const { data: productsData, isPending: isLoading, error } = useQuery({
    queryKey: ["products", params],
    queryFn: () => getProducts(params),
  });

  return { productsData, isLoading, error };
}
