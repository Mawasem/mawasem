import { LoaderCircle, Minus, Plus, Trash2 } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import type { CartItem } from "../types/cart.types"

interface CartItemCardProps {
  item: CartItem
  busy: boolean
  onQuantityChange: (quantity: number) => void
  onRemove: () => void
}

export function CartItemCard({
  item,
  busy,
  onQuantityChange,
  onRemove,
}: CartItemCardProps) {
  return (
    <Card>
      <CardContent className="grid gap-4 p-5 sm:grid-cols-[1fr_auto] sm:items-center">
        <div className="min-w-0 space-y-2">
          <div>
            <h2 className="font-semibold">
              {item.productNameEn || item.productNameAr}
            </h2>
            <p className="text-sm text-muted-foreground">{item.sku}</p>
            {item.optionCombinationKey ? (
              <p className="mt-1 text-sm text-muted-foreground">
                {item.optionCombinationKey}
              </p>
            ) : null}
          </div>
          {item.warnings.map((warning) => (
            <p key={warning.code} className="text-sm text-destructive">
              {warning.message}
            </p>
          ))}
          <p className="font-medium">EGP {item.currentUnitPrice.toFixed(2)}</p>
        </div>

        <div className="flex flex-wrap items-center justify-between gap-3 sm:flex-col sm:items-end">
          <div className="flex items-center rounded-md border">
            <Button
              type="button"
              variant="ghost"
              size="icon"
              disabled={busy || item.quantity <= 1}
              onClick={() => onQuantityChange(item.quantity - 1)}
            >
              <Minus className="size-4" />
              <span className="sr-only">Decrease quantity</span>
            </Button>
            <span className="w-10 text-center text-sm font-medium">
              {item.quantity}
            </span>
            <Button
              type="button"
              variant="ghost"
              size="icon"
              disabled={busy || item.quantity >= item.stockQuantity}
              onClick={() => onQuantityChange(item.quantity + 1)}
            >
              <Plus className="size-4" />
              <span className="sr-only">Increase quantity</span>
            </Button>
          </div>
          <p className="font-semibold">EGP {item.lineTotal.toFixed(2)}</p>
          <Button
            type="button"
            variant="ghost"
            size="sm"
            className="text-destructive hover:text-destructive"
            disabled={busy}
            onClick={onRemove}
          >
            {busy ? (
              <LoaderCircle className="size-4 animate-spin" />
            ) : (
              <Trash2 className="size-4" />
            )}{" "}
            Remove
          </Button>
        </div>
      </CardContent>
    </Card>
  )
}
