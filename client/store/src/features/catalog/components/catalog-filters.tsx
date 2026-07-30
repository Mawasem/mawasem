import { RotateCcw } from "lucide-react"

import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"

interface CatalogFiltersProps {
  minimumPrice: string
  maximumPrice: string
  inStockOnly: boolean
  isFeatured: boolean
  onChange: (key: string, value: string | boolean) => void
  onClear: () => void
}

export function CatalogFilters({
  minimumPrice,
  maximumPrice,
  inStockOnly,
  isFeatured,
  onChange,
  onClear,
}: CatalogFiltersProps) {
  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="font-semibold">Filters</h2>
        <Button type="button" variant="ghost" size="sm" onClick={onClear}>
          <RotateCcw className="size-4" />
          Reset
        </Button>
      </div>

      <Separator />

      <div className="space-y-3">
        <h3 className="text-sm font-medium">Price range</h3>
        <div className="grid grid-cols-2 gap-2">
          <div className="space-y-1.5">
            <Label htmlFor="minimum-price" className="text-xs">
              Minimum
            </Label>
            <Input
              id="minimum-price"
              type="number"
              min="0"
              value={minimumPrice}
              onChange={(event) => onChange("minPrice", event.target.value)}
              placeholder="0"
            />
          </div>
          <div className="space-y-1.5">
            <Label htmlFor="maximum-price" className="text-xs">
              Maximum
            </Label>
            <Input
              id="maximum-price"
              type="number"
              min="0"
              value={maximumPrice}
              onChange={(event) => onChange("maxPrice", event.target.value)}
              placeholder="Any"
            />
          </div>
        </div>
      </div>

      <Separator />

      <div className="space-y-4">
        <label className="flex cursor-pointer items-center gap-3 text-sm">
          <Checkbox
            checked={inStockOnly}
            onCheckedChange={(checked) => onChange("inStock", checked === true)}
          />
          In-stock products only
        </label>

        <label className="flex cursor-pointer items-center gap-3 text-sm">
          <Checkbox
            checked={isFeatured}
            onCheckedChange={(checked) =>
              onChange("featured", checked === true)
            }
          />
          Featured products only
        </label>
      </div>
    </div>
  )
}
