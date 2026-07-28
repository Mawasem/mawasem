import type { ColumnDef } from "@tanstack/react-table";
import type { ReactNode } from "react";

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

export interface EntityManagementPagePagination {
  totalCount: number;
  page: number;
  totalPages: number;
  totalCountLabel?: string;
  pageLabel?: string;
  previousLabel?: string;
  nextLabel?: string;
  onPageChange: (page: number) => void;
}

export interface EntityManagementPageProps<TData, TValue> {
  title: string;
  description: string;
  search: string;
  onSearch: (value: string) => void;
  includeDeleted?: boolean;
  onIncludeDeletedChange?: (value: boolean) => void;
  includeDeletedLabel?: string;
  includeDeletedSwitchId?: string;
  buttonLabel: string;
  onCreate: () => void;
  searchPlaceholder?: string;
  columns: ColumnDef<TData, TValue>[];
  data: TData[];
  emptyStateLabel?: string;
  loading: boolean;
  loadingLabel?: string;
  error: unknown;
  errorRenderer?: (error: Error) => string;
  pagination: EntityManagementPagePagination;
  filtersSlot?: ReactNode;
  children?: ReactNode;
}