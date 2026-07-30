import { PackageSearch } from "lucide-react"

import { Button } from "@/components/ui/button"

interface CatalogEmptyStateProps {
  onClear: () => void
}

export function CatalogEmptyState({ onClear }: CatalogEmptyStateProps) {
  return (
    <div className="grid min-h-80 place-items-center rounded-xl border border-dashed p-8 text-center">
      <div className="space-y-4">
        <PackageSearch className="mx-auto size-10 text-muted-foreground" />
        <div>
          <h2 className="font-semibold">No products found</h2>
          <p className="mt-1 text-sm text-muted-foreground">
            Try changing your search or filters.
          </p>
        </div>
        <Button variant="outline" onClick={onClear}>
          Clear filters
        </Button>
      </div>
    </div>
  )
}
