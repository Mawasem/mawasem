import type { PublicProductListItem } from "../types/product.types"
import { ProductCard } from "./product-card"
import { ProductCardSkeleton } from "./product-card-skeleton"

interface ProductGridProps {
  products: PublicProductListItem[]
  isLoading: boolean
}

export function ProductGrid({ products, isLoading }: ProductGridProps) {
  if (isLoading) {
    return (
      <div className="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
        {Array.from({ length: 6 }).map((_, index) => (
          <ProductCardSkeleton key={index} />
        ))}
      </div>
    )
  }

  return (
    <div className="grid gap-5 sm:grid-cols-2 xl:grid-cols-3">
      {products.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  )
}
