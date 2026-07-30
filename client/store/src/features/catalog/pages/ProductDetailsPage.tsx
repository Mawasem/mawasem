import { ArrowLeft, Star } from "lucide-react"
import { Link, useParams } from "react-router-dom"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { Skeleton } from "@/components/ui/skeleton"

import { ProductPurchasePanel } from "../components/product-purchase-panel"
import { usePublicProduct } from "../hooks/use-public-product"
import { getLocalizedValue } from "../utils/get-localized-value"
import { resolveMediaUrl } from "../utils/resolve-media-url"

export default function ProductDetailsPage() {
  const { slug = "" } = useParams()
  const { productData, isLoading, error } = usePublicProduct(slug)

  if (isLoading) {
    return (
      <div className="grid gap-8 lg:grid-cols-2">
        <Skeleton className="aspect-square w-full" />
        <div className="space-y-4">
          <Skeleton className="h-8 w-2/3" />
          <Skeleton className="h-5 w-1/3" />
          <Skeleton className="h-24 w-full" />
        </div>
      </div>
    )
  }

  if (error || !productData) {
    return (
      <div className="space-y-4 rounded-xl border p-8 text-center">
        <h1 className="text-xl font-semibold">Product not found</h1>
        <Button asChild variant="outline">
          <Link to="/seasons/back-to-school">Back to products</Link>
        </Button>
      </div>
    )
  }

  const product = productData
  const name = getLocalizedValue("en", product.nameEn, product.nameAr)
  const description = getLocalizedValue(
    "en",
    product.descriptionEn,
    product.descriptionAr
  )
  const mainImage = resolveMediaUrl(
    product.images.find((image) => image.isPrimary)?.imageUrl ??
      product.primaryImageUrl
  )

  return (
    <section className="space-y-8">
      <Button asChild variant="ghost" className="px-0">
        <Link to={`/seasons/back-to-school`}>
          <ArrowLeft className="size-4" />
          Back to products
        </Link>
      </Button>

      <div className="grid gap-8 lg:grid-cols-2">
        <Card className="overflow-hidden py-0">
          <div className="aspect-square bg-muted">
            {mainImage ? (
              <img
                src={mainImage}
                alt={name}
                className="size-full object-cover"
              />
            ) : (
              <div className="grid size-full place-items-center text-muted-foreground">
                No image available
              </div>
            )}
          </div>
        </Card>

        <div className="space-y-6">
          <div className="space-y-3">
            <div className="flex flex-wrap gap-2">
              {product.isFeatured ? <Badge>Featured</Badge> : null}
              <Badge variant={product.isInStock ? "secondary" : "destructive"}>
                {product.isInStock ? "In stock" : "Out of stock"}
              </Badge>
            </div>
            <p className="text-sm font-medium text-muted-foreground">
              {getLocalizedValue(
                "en",
                product.brand.nameEn,
                product.brand.nameAr
              )}
            </p>
            <h1 className="text-3xl font-bold tracking-tight">{name}</h1>
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <Star className="size-4 fill-current" />
              {product.averageRating.toFixed(1)} ({product.reviewCount} reviews)
            </div>
          </div>

          <div className="flex items-end gap-3">
            <span className="text-3xl font-bold">
              EGP {product.currentPrice.toFixed(2)}
            </span>
            {product.originalPrice > product.currentPrice ? (
              <span className="pb-1 text-muted-foreground line-through">
                EGP {product.originalPrice.toFixed(2)}
              </span>
            ) : null}
          </div>

          <p className="leading-7 text-muted-foreground">{description}</p>

          <Separator />

          <ProductPurchasePanel product={product} />
        </div>
      </div>

      {product.specifications.length > 0 ? (
        <Card>
          <CardContent className="space-y-4">
            <h2 className="text-xl font-semibold">Specifications</h2>
            <div className="divide-y rounded-lg border">
              {product.specifications.map((specification) => (
                <div
                  key={specification.id}
                  className="grid gap-1 p-4 sm:grid-cols-2"
                >
                  <span className="font-medium">
                    {getLocalizedValue(
                      "en",
                      specification.nameEn,
                      specification.nameAr
                    )}
                  </span>
                  <span className="text-muted-foreground">
                    {getLocalizedValue(
                      "en",
                      specification.valueEn,
                      specification.valueAr
                    )}
                  </span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      ) : null}
    </section>
  )
}
