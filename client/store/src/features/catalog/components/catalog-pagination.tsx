import { ChevronLeft, ChevronRight } from "lucide-react"

import { Button } from "@/components/ui/button"

interface CatalogPaginationProps {
  pageNumber: number
  totalPages: number
  onPageChange: (page: number) => void
}

export function CatalogPagination({
  pageNumber,
  totalPages,
  onPageChange,
}: CatalogPaginationProps) {
  if (totalPages <= 1) return null

  return (
    <nav
      className="flex items-center justify-center gap-3"
      aria-label="Products pagination"
    >
      <Button
        type="button"
        variant="outline"
        size="icon"
        disabled={pageNumber <= 1}
        onClick={() => onPageChange(pageNumber - 1)}
      >
        <ChevronLeft className="size-4" />
        <span className="sr-only">Previous page</span>
      </Button>

      <span className="text-sm text-muted-foreground">
        Page <strong className="text-foreground">{pageNumber}</strong> of{" "}
        {totalPages}
      </span>

      <Button
        type="button"
        variant="outline"
        size="icon"
        disabled={pageNumber >= totalPages}
        onClick={() => onPageChange(pageNumber + 1)}
      >
        <ChevronRight className="size-4" />
        <span className="sr-only">Next page</span>
      </Button>
    </nav>
  )
}
