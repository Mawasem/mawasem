import { Button } from "@/components/ui/button";
import type { EntityPaginationProps } from "./types";

export function EntityPagination({
  totalCount,
  page,
  totalPages,
  totalCountLabel = "row(s)",
  pageLabel = "Page",
  previousLabel = "Previous",
  nextLabel = "Next",
  onPageChange,
}: EntityPaginationProps) {
  const safeTotalPages =
    totalPages > 0 ? totalPages : 1;

  return (
    <div className="flex items-center justify-between px-2">
      <div className="flex-1 text-sm text-muted-foreground">
        {totalCount} {totalCountLabel}
      </div>

      <div className="flex items-center space-x-6 lg:space-x-8">
        <div className="text-sm font-medium">
          {pageLabel} {page} of {safeTotalPages}
        </div>

        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            size="sm"
            disabled={page <= 1}
            onClick={() =>
              onPageChange(page - 1)
            }
          >
            {previousLabel}
          </Button>

          <Button
            variant="outline"
            size="sm"
            disabled={page >= safeTotalPages}
            onClick={() =>
              onPageChange(page + 1)
            }
          >
            {nextLabel}
          </Button>
        </div>
      </div>
    </div>
  );
}
