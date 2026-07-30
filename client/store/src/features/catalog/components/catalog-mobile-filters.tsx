import { SlidersHorizontal } from "lucide-react"
import type { ComponentProps } from "react"

import { Button } from "@/components/ui/button"
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet"

import { CatalogFilters } from "./catalog-filters"

type CatalogMobileFiltersProps = ComponentProps<typeof CatalogFilters>

export function CatalogMobileFilters(props: CatalogMobileFiltersProps) {
  return (
    <Sheet>
      <SheetTrigger asChild>
        <Button type="button" variant="outline" className="lg:hidden">
          <SlidersHorizontal className="size-4" />
          Filters
        </Button>
      </SheetTrigger>
      <SheetContent side="left" className="overflow-y-auto p-6">
        <SheetHeader className="p-0 text-start">
          <SheetTitle>Product filters</SheetTitle>
          <SheetDescription>
            Refine the products shown on this page.
          </SheetDescription>
        </SheetHeader>
        <div className="mt-6">
          <CatalogFilters {...props} />
        </div>
      </SheetContent>
    </Sheet>
  )
}
