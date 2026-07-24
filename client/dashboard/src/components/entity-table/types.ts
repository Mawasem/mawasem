import type { ColumnDef } from "@tanstack/react-table";

export interface EntityTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[];
  data: TData[];
  emptyStateLabel?: string;
}


export interface EntityToolbarProps {
  search: string;
  onSearch: (value: string) => void;
  searchPlaceholder?: string;

  buttonText: string;
  onAdd: () => void;
}

export interface EntityPaginationProps {
  totalCount: number;
  page: number;
  totalPages: number;
  totalCountLabel?: string;
  pageLabel?: string;
  previousLabel?: string;
  nextLabel?: string;
  onPageChange: (
    page: number
  ) => void;
}
