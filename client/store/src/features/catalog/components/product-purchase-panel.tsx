import { LoaderCircle, Minus, Plus, ShoppingCart } from "lucide-react"
import { useMemo, useState } from "react"
import { Button } from "@/components/ui/button"
import { getApiErrorMessage } from "@/lib/get-api-error-message"
import { useAddToCart } from "@/features/cart/hooks/use-add-to-cart"
import type { PublicProductDetails } from "../types/product-details.types"
import { getLocalizedValue } from "../utils/get-localized-value"

export function ProductPurchasePanel({
  product,
}: {
  product: PublicProductDetails
}) {
  const [selectedValues, setSelectedValues] = useState<Record<number, number>>(
    {}
  )
  const [quantity, setQuantity] = useState(1)
  const mutation = useAddToCart()
  const selectedVariant = useMemo(
    () =>
      product.variants.find((variant) =>
        product.options.every((option) =>
          variant.options.some(
            (value) =>
              value.optionId === option.id &&
              value.optionValueId === selectedValues[option.id]
          )
        )
      ),
    [product.options, product.variants, selectedValues]
  )
  const allSelected = product.options.every((option) =>
    Boolean(selectedValues[option.id])
  )
  const canAdd =
    product.options.length === 0
      ? product.variants.length === 1 && product.variants[0].canPurchase
      : Boolean(selectedVariant?.canPurchase) && allSelected
  const variant =
    product.options.length === 0 ? product.variants[0] : selectedVariant
  const max = variant?.stockQuantity ?? 1
  async function add() {
    if (!variant) return
    await mutation.addToCartAsync({ productVariantId: variant.id, quantity })
    setQuantity(1)
  }
  return (
    <div className="space-y-5">
      {product.options.map((option) => (
        <div key={option.id} className="space-y-2">
          <h2 className="text-sm font-medium">
            {getLocalizedValue("en", option.nameEn, option.nameAr)}
          </h2>
          <div className="flex flex-wrap gap-2">
            {option.values.map((value) => {
              const selected = selectedValues[option.id] === value.id
              return (
                <Button
                  key={value.id}
                  type="button"
                  variant={selected ? "default" : "outline"}
                  size="sm"
                  onClick={() =>
                    setSelectedValues((current) => ({
                      ...current,
                      [option.id]: value.id,
                    }))
                  }
                >
                  {getLocalizedValue("en", value.valueEn, value.valueAr)}
                </Button>
              )
            })}
          </div>
        </div>
      ))}
      <div className="flex items-center justify-between gap-4">
        <span className="text-sm font-medium">Quantity</span>
        <div className="flex items-center rounded-md border">
          <Button
            type="button"
            variant="ghost"
            size="icon"
            disabled={quantity <= 1 || mutation.isLoading}
            onClick={() => setQuantity((q) => q - 1)}
          >
            <Minus className="size-4" />
          </Button>
          <span className="w-10 text-center">{quantity}</span>
          <Button
            type="button"
            variant="ghost"
            size="icon"
            disabled={quantity >= max || mutation.isLoading}
            onClick={() => setQuantity((q) => q + 1)}
          >
            <Plus className="size-4" />
          </Button>
        </div>
      </div>
      {mutation.error ? (
        <p className="text-sm text-destructive">
          {getApiErrorMessage(
            mutation.error,
            "Could not add this product to the cart."
          )}
        </p>
      ) : null}
      {mutation.isSuccess ? (
        <p className="text-sm text-primary">Added to cart.</p>
      ) : null}
      <Button
        size="lg"
        className="w-full"
        disabled={!canAdd || mutation.isLoading}
        onClick={() => void add()}
      >
        {mutation.isLoading ? (
          <LoaderCircle className="size-5 animate-spin" />
        ) : (
          <ShoppingCart className="size-5" />
        )}
        {canAdd ? "Add to cart" : "Select an available variant"}
      </Button>
    </div>
  )
}
