import { useQuery } from "@tanstack/react-query"

import { getPublicProducts } from "../api/get-public-products"
import { catalogQueryKeys } from "../query-keys/catalog-query-keys"
import type { GetPublicProductsParams } from "../types/product-query.types"

export function usePublicProducts(params: GetPublicProductsParams) {
  const {
    data: productsData,
    isPending: isLoading,
    isFetching,
    error,
  } = useQuery({
    queryKey: catalogQueryKeys.list(params),
    queryFn: () => getPublicProducts(params),
    placeholderData: (previousData) => previousData,
  })

  return { productsData, isLoading, isFetching, error }
}
