import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"

import { PublicProductSortOption } from "../types/product-query.types"

interface CatalogSortSelectProps {
  value: PublicProductSortOption
  onChange: (value: PublicProductSortOption) => void
}

export function CatalogSortSelect({ value, onChange }: CatalogSortSelectProps) {
  return (
    <Select
      value={String(value)}
      onValueChange={(nextValue) =>
        onChange(Number(nextValue) as PublicProductSortOption)
      }
    >
      <SelectTrigger className="w-full sm:w-52">
        <SelectValue placeholder="Sort products" />
      </SelectTrigger>
      <SelectContent>
        <SelectItem value={String(PublicProductSortOption.Newest)}>
          Newest
        </SelectItem>
        <SelectItem value={String(PublicProductSortOption.PriceLowToHigh)}>
          Price: low to high
        </SelectItem>
        <SelectItem value={String(PublicProductSortOption.PriceHighToLow)}>
          Price: high to low
        </SelectItem>
      </SelectContent>
    </Select>
  )
}
