import { useMemo } from "react"
import { useSearchParams } from "react-router-dom"

import { PublicProductSortOption } from "../types/product-query.types"

function parsePositiveNumber(value: string | null) {
  if (!value) return undefined
  const parsed = Number(value)
  return Number.isFinite(parsed) && parsed >= 0 ? parsed : undefined
}

export function useCatalogSearchParams() {
  const [searchParams, setSearchParams] = useSearchParams()

  const filters = useMemo(
    () => ({
      searchTerm: searchParams.get("search") ?? "",
      minimumPrice: searchParams.get("minPrice") ?? "",
      maximumPrice: searchParams.get("maxPrice") ?? "",
      inStockOnly: searchParams.get("inStock") === "true",
      isFeatured: searchParams.get("featured") === "true",
      sortBy:
        (Number(searchParams.get("sort")) as PublicProductSortOption) ||
        PublicProductSortOption.Newest,
      pageNumber: Math.max(Number(searchParams.get("page")) || 1, 1),
    }),
    [searchParams]
  )

  const setFilter = (
    key: string,
    value: string | number | boolean | undefined
  ) => {
    const next = new URLSearchParams(searchParams)

    if (value === undefined || value === "" || value === false) {
      next.delete(key)
    } else {
      next.set(key, String(value))
    }

    if (key !== "page") next.delete("page")
    setSearchParams(next)
  }

  const clearFilters = () => setSearchParams({})

  return {
    filters,
    minimumPrice: parsePositiveNumber(filters.minimumPrice),
    maximumPrice: parsePositiveNumber(filters.maximumPrice),
    setFilter,
    clearFilters,
  }
}
