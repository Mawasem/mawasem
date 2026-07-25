
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { EntityPagination } from "@/components/entity-table/entity-pagination";
import { EntityTable } from "@/components/entity-table/entity-table";
import { EntityToolbar } from "@/components/entity-table/entity-toolbar";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { CollectionDialog } from "../components/collection-dialog";
import { useCollectionColumns } from "../components/collection-columns";
import { useCollections } from "../hooks/use-collections";

export default function CategoriesPage() {
  const { t } = useTranslation();

  const collectionColumns = useCollectionColumns();

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
  } = useCollections({
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

  const handleAddCollection = () => {
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
        searchPlaceholder={t("collections.searchPlaceholder")}
        buttonText={t("collections.actions.create")}
        onAdd={handleAddCollection}
      />

      <div className="flex items-center gap-2">
        <Switch
          id="include-deleted-collections"
          checked={includeDeleted}
          onCheckedChange={handleIncludeDeletedChange}
        />

        <Label htmlFor="include-deleted-collections">
          {t("collections.filters.includeDeleted")}
        </Label>
      </div>

      <EntityTable
        columns={collectionColumns}
        data={data?.items ?? []}
        emptyStateLabel={t("collections.empty")}
      />

      {isLoading ? (
        <p className="text-sm text-muted-foreground">
          {t("collections.loading")}
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
        totalCountLabel={t("collections.pagination.rows")}
        pageLabel={t("collections.pagination.page")}
        previousLabel={t("collections.pagination.previous")}
        nextLabel={t("collections.pagination.next")}
        onPageChange={handlePageChange}
      />

      <CollectionDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </div>
  );
}
