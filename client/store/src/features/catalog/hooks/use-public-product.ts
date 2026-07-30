import { useQuery } from "@tanstack/react-query"

import { getPublicProductBySlug } from "../api/get-public-product-by-slug"
import { catalogQueryKeys } from "../query-keys/catalog-query-keys"

export function usePublicProduct(slug: string) {
  const {
    data: productData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: catalogQueryKeys.detail(slug),
    queryFn: () => getPublicProductBySlug(slug),
    enabled: Boolean(slug),
  })

  return { productData, isLoading, error }
}
