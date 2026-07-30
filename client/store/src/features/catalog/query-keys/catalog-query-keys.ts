import type { GetPublicProductsParams } from "../types/product-query.types"

export const catalogQueryKeys = {
  all: ["public-products"] as const,
  lists: () => [...catalogQueryKeys.all, "list"] as const,
  list: (params: GetPublicProductsParams) =>
    [...catalogQueryKeys.lists(), params] as const,
  details: () => [...catalogQueryKeys.all, "detail"] as const,
  detail: (slug: string) => [...catalogQueryKeys.details(), slug] as const,
}
