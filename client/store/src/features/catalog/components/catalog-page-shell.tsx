import { LoaderCircle } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"

import { CATALOG_PAGE_SIZE } from "../constants/catalog.constants"
import { useCatalogSearchParams } from "../hooks/use-catalog-search-params"
import { usePublicProducts } from "../hooks/use-public-products"
import type { CatalogPageConfig } from "../types/catalog-page.types"
import { CatalogEmptyState } from "./catalog-empty-state"
import { CatalogErrorState } from "./catalog-error-state"
import { CatalogFilters } from "./catalog-filters"
import { CatalogMobileFilters } from "./catalog-mobile-filters"
import { CatalogPagination } from "./catalog-pagination"
import { CatalogSearch } from "./catalog-search"
import { CatalogSortSelect } from "./catalog-sort-select"
import { ProductGrid } from "./product-grid"

interface CatalogPageShellProps {
  config: CatalogPageConfig
}

export function CatalogPageShell({ config }: CatalogPageShellProps) {
  const { filters, minimumPrice, maximumPrice, setFilter, clearFilters } =
    useCatalogSearchParams()

  const { productsData, isLoading, isFetching, error } = usePublicProducts({
    searchTerm: filters.searchTerm || undefined,
    seasonId: config.seasonId,
    minimumPrice,
    maximumPrice,
    inStockOnly: filters.inStockOnly,
    isFeatured: filters.isFeatured ? true : undefined,
    sortBy: filters.sortBy,
    pageNumber: filters.pageNumber,
    pageSize: CATALOG_PAGE_SIZE,
  })

  const filterProps = {
    minimumPrice: filters.minimumPrice,
    maximumPrice: filters.maximumPrice,
    inStockOnly: filters.inStockOnly,
    isFeatured: filters.isFeatured,
    onChange: setFilter,
    onClear: clearFilters,
  }

  return (
    <section className="space-y-8">
      <header className="space-y-3">
        <div className="flex flex-wrap items-center gap-3">
          <h1 className="text-3xl font-bold tracking-tight md:text-4xl">
            {config.title}
          </h1>
          {isFetching && !isLoading ? (
            <LoaderCircle className="size-5 animate-spin text-muted-foreground" />
          ) : null}
        </div>
        {config.description ? (
          <p className="max-w-3xl text-muted-foreground">
            {config.description}
          </p>
        ) : null}
      </header>

      <div className="grid gap-4 md:grid-cols-[1fr_auto]">
        <CatalogSearch
          value={filters.searchTerm}
          onSearch={(value) => setFilter("search", value)}
        />
        <div className="flex gap-2">
          <CatalogMobileFilters {...filterProps} />
          <CatalogSortSelect
            value={filters.sortBy}
            onChange={(value) => setFilter("sort", value)}
          />
        </div>
      </div>

      <div className="grid gap-8 lg:grid-cols-[260px_minmax(0,1fr)]">
        <aside className="hidden lg:block">
          <Card className="sticky top-24 py-5">
            <CardContent className="px-5">
              <CatalogFilters {...filterProps} />
            </CardContent>
          </Card>
        </aside>

        <div className="space-y-6">
          <div className="flex items-center justify-between">
            <p className="text-sm text-muted-foreground">
              {productsData
                ? `${productsData.totalCount} products`
                : "Loading products..."}
            </p>
            {filters.inStockOnly || filters.isFeatured ? (
              <Badge variant="secondary">Filtered</Badge>
            ) : null}
          </div>

          {error ? (
            <CatalogErrorState error={error} />
          ) : productsData && productsData.items.length === 0 && !isLoading ? (
            <CatalogEmptyState onClear={clearFilters} />
          ) : (
            <ProductGrid
              products={productsData?.items ?? []}
              isLoading={isLoading}
            />
          )}

          <CatalogPagination
            pageNumber={productsData?.pageNumber ?? filters.pageNumber}
            totalPages={productsData?.totalPages ?? 0}
            onPageChange={(page) => setFilter("page", page)}
          />
        </div>
      </div>
    </section>
  )
}
