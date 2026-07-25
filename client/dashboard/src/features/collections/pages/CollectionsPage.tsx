
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage";
import { CollectionDialog } from "../components/collection-dialog";
import { useCollectionColumns } from "../components/collection-columns";
import { useCollections } from "../hooks/use-collections";

export default function CollectionsPage() {
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
    <EntityManagementPage
      title={t("collections.page.title")}
      description={t("collections.page.description")}
      search={search}
      onSearch={handleSearch}
      includeDeleted={includeDeleted}
      onIncludeDeletedChange={handleIncludeDeletedChange}
      includeDeletedLabel={t("collections.filters.includeDeleted")}
      includeDeletedSwitchId="include-deleted-collections"
      buttonLabel={t("collections.actions.create")}
      onCreate={handleAddCollection}
      columns={collectionColumns}
      data={data?.items ?? []}
      emptyStateLabel={t("collections.empty")}
      loading={isLoading}
      loadingLabel={t("collections.loading")}
      error={error}
      pagination={{
        totalCount,
        page: currentPage,
        totalPages,
        totalCountLabel: t("collections.pagination.rows"),
        pageLabel: t("collections.pagination.page"),
        previousLabel: t("collections.pagination.previous"),
        nextLabel: t("collections.pagination.next"),
        onPageChange: handlePageChange,
      }}
      searchPlaceholder={t("collections.searchPlaceholder")}
    >
      <CollectionDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </EntityManagementPage>
  );
}
