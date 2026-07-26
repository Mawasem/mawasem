import type { ReactNode } from "react";
import type { ColumnDef } from "@tanstack/react-table";

import { EntityPagination } from "@/components/entity-table/entity-pagination";
import { EntityTable } from "@/components/entity-table/entity-table";
import { EntityToolbar } from "@/components/entity-table/entity-toolbar";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";

interface EntityManagementPagePagination {
  totalCount: number;
  page: number;
  totalPages: number;
  totalCountLabel?: string;
  pageLabel?: string;
  previousLabel?: string;
  nextLabel?: string;
  onPageChange: (page: number) => void;
}

interface EntityManagementPageProps<TData, TValue> {
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

export function EntityManagementPage<TData, TValue>({
  title,
  description,
  search,
  onSearch,
  includeDeleted,
  onIncludeDeletedChange,
  includeDeletedLabel,
  includeDeletedSwitchId,
  buttonLabel,
  onCreate,
  searchPlaceholder,
  columns,
  data,
  emptyStateLabel,
  loading,
  loadingLabel,
  error,
  errorRenderer,
  pagination,
  filtersSlot,
  children,
}: EntityManagementPageProps<TData, TValue>) {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">
          {title}
        </h1>

        <p className="text-muted-foreground">
          {description}
        </p>
      </div>

      <EntityToolbar
        search={search}
        onSearch={onSearch}
        searchPlaceholder={searchPlaceholder}
        buttonText={buttonLabel}
        onAdd={onCreate}
      />

      <div
        className={`flex items-center ${
          filtersSlot ? "gap-6" : "gap-2"
        }`}
      >
        {filtersSlot}

        {typeof includeDeleted === "boolean" &&
        onIncludeDeletedChange &&
        includeDeletedLabel &&
        includeDeletedSwitchId ? (
          <div className="flex items-center gap-2">
            <Switch
              id={includeDeletedSwitchId}
              checked={includeDeleted}
              onCheckedChange={
                onIncludeDeletedChange
              }
            />

            <Label htmlFor={includeDeletedSwitchId}>
              {includeDeletedLabel}
            </Label>
          </div>
        ) : null}
      </div>

      <EntityTable
        columns={columns}
        data={data}
        emptyStateLabel={emptyStateLabel}
      />

      {loading && loadingLabel ? (
        <p className="text-sm text-muted-foreground">
          {loadingLabel}
        </p>
      ) : null}

      {error instanceof Error ? (
        <p className="text-sm text-destructive">
          {errorRenderer
            ? errorRenderer(error)
            : error.message}
        </p>
      ) : null}

      <EntityPagination
        totalCount={pagination.totalCount}
        page={pagination.page}
        totalPages={pagination.totalPages}
        totalCountLabel={pagination.totalCountLabel}
        pageLabel={pagination.pageLabel}
        previousLabel={pagination.previousLabel}
        nextLabel={pagination.nextLabel}
        onPageChange={pagination.onPageChange}
      />

      {children}
    </div>
  );
}
