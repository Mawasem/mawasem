
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { EntityPagination } from "@/components/entity-table/entity-pagination";
import { EntityTable } from "@/components/entity-table/entity-table";
import { EntityToolbar } from "@/components/entity-table/entity-toolbar";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { CategoryDialog } from "../components/category-dialog";
import { useCategoryColumns } from "../components/category-columns";
import { useCategories } from "../hooks/use-categories";

export default function CategoriesPage() {
  const { t } = useTranslation();

  const categoryColumns = useCategoryColumns();

  const [search, setSearch] = useState("");
  const [includeDeleted, setIncludeDeleted] =
    useState(false);
  const [requestedPageNumber, setRequestedPageNumber] =
    useState(1);
  const [isCreateDialogOpen, setIsCreateDialogOpen] =
    useState(false);

  const {
    data,
    isLoading,
    error,
  } = useCategories({
    search,
    includeDeleted,
    pageNumber: requestedPageNumber,
    pageSize: 10,
  });

  const currentPage =
    data?.pageNumber ?? requestedPageNumber;

  const totalPages =
    data?.totalPages ?? 1;

  const totalCount =
    data?.totalCount ?? 0;

  const handleSearch = (value: string) => {
    setSearch(value);
    setRequestedPageNumber(1);
  };

  const handleIncludeDeletedChange = (
    value: boolean
  ) => {
    setIncludeDeleted(value);
    setRequestedPageNumber(1);
  };

  const handlePageChange = (
    nextPage: number
  ) => {
    if (
      nextPage < 1 ||
      nextPage > totalPages ||
      nextPage === currentPage
    ) {
      return;
    }

    setRequestedPageNumber(nextPage);
  };

  const handleAddCategory = () => {
    setIsCreateDialogOpen(true);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">
          {t("categories.page.title")}
        </h1>

        <p className="text-muted-foreground">
          {t("categories.page.description")}
        </p>
      </div>

      <EntityToolbar
        search={search}
        onSearch={handleSearch}
        searchPlaceholder={t("categories.searchPlaceholder")}
        buttonText={t("categories.actions.create")}
        onAdd={handleAddCategory}
      />

      <div className="flex items-center gap-2">
        <Switch
          id="include-deleted-categories"
          checked={includeDeleted}
          onCheckedChange={
            handleIncludeDeletedChange
          }
        />

        <Label htmlFor="include-deleted-categories">
          {t("categories.filters.includeDeleted")}
        </Label>
      </div>

      <EntityTable
        columns={categoryColumns}
        data={data?.items ?? []}
        emptyStateLabel={t("categories.empty")}
      />

      {isLoading ? (
        <p className="text-sm text-muted-foreground">
          {t("categories.loading")}
        </p>
      ) : null}

      {error instanceof Error ? (
        <p className="text-sm text-destructive">
          {error.message}
        </p>
      ) : null}

      <EntityPagination
        totalCount={totalCount}
        page={currentPage}
        totalPages={totalPages}
        totalCountLabel={t("categories.pagination.rows")}
        pageLabel={t("categories.pagination.page")}
        previousLabel={t("categories.pagination.previous")}
        nextLabel={t("categories.pagination.next")}
        onPageChange={handlePageChange}
      />

      <CategoryDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </div>
  );
}
