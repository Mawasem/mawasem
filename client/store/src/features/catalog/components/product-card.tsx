import { Heart, ShoppingCart } from "lucide-react"
import { Link } from "react-router-dom"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardFooter } from "@/components/ui/card"

import type { PublicProductListItem } from "../types/product.types"
import { getLocalizedValue } from "../utils/get-localized-value"
import { resolveMediaUrl } from "../utils/resolve-media-url"

interface ProductCardProps {
  product: PublicProductListItem
  language?: string
}

export function ProductCard({ product, language = "en" }: ProductCardProps) {
  const name = getLocalizedValue(language, product.nameEn, product.nameAr)
  const brandName = getLocalizedValue(
    language,
    product.brand.nameEn,
    product.brand.nameAr
  )
  const imageUrl = resolveMediaUrl(product.primaryImageUrl)

  return (
    <Card className="group overflow-hidden py-0 transition-shadow hover:shadow-md">
      <Link
        to={`/products/${product.slug}`}
        className="relative block aspect-square overflow-hidden bg-muted"
      >
        {imageUrl ? (
          <img
            src={imageUrl}
            alt={name}
            className="size-full object-cover transition-transform duration-300 group-hover:scale-105"
            loading="lazy"
          />
        ) : (
          <div className="grid size-full place-items-center text-sm text-muted-foreground">
            No image
          </div>
        )}

        <div className="absolute start-3 top-3 flex flex-wrap gap-2">
          {product.discountPercentage > 0 ? (
            <Badge variant="destructive">
              -{Math.round(product.discountPercentage)}%
            </Badge>
          ) : null}
          {product.isFeatured ? <Badge>Featured</Badge> : null}
        </div>

        <Button
          type="button"
          size="icon"
          variant="secondary"
          className="absolute end-3 top-3 rounded-full opacity-0 transition-opacity group-hover:opacity-100"
          onClick={(event) => event.preventDefault()}
          aria-label={`Add ${name} to wishlist`}
        >
          <Heart className="size-4" />
        </Button>
      </Link>

      <CardContent className="space-y-2 p-4">
        <p className="text-xs font-medium tracking-wide text-muted-foreground uppercase">
          {brandName}
        </p>
        <Link
          to={`/products/${product.slug}`}
          className="line-clamp-2 font-semibold hover:text-primary"
        >
          {name}
        </Link>
        <div className="flex items-center gap-2">
          <span className="font-bold">
            EGP {product.currentPrice.toFixed(2)}
          </span>
          {product.originalPrice > product.currentPrice ? (
            <span className="text-sm text-muted-foreground line-through">
              EGP {product.originalPrice.toFixed(2)}
            </span>
          ) : null}
        </div>
        <p className="text-xs text-muted-foreground">
          {product.isInStock ? "In stock" : "Out of stock"}
        </p>
      </CardContent>

      <CardFooter className="p-4 pt-0">
        <Button className="w-full" disabled={!product.canPurchase}>
          <ShoppingCart className="size-4" />
          {product.canPurchase ? "Add to cart" : "Unavailable"}
        </Button>
      </CardFooter>
    </Card>
  )
}
