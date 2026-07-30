import { PublicProductSortOption } from "../types/product-query.types"

export const CATALOG_PAGE_SIZE = 12

export const DEFAULT_CATALOG_FILTERS = {
  searchTerm: "",
  minimumPrice: "",
  maximumPrice: "",
  inStockOnly: false,
  isFeatured: false,
  sortBy: PublicProductSortOption.Newest,
  pageNumber: 1,
} as const
