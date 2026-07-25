
import { useState } from "react";
import { useTranslation } from "react-i18next";

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage";
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
    <EntityManagementPage
      title={t("categories.page.title")}
      description={t("categories.page.description")}
      search={search}
      onSearch={handleSearch}
      includeDeleted={includeDeleted}
      onIncludeDeletedChange={handleIncludeDeletedChange}
      includeDeletedLabel={t("categories.filters.includeDeleted")}
      includeDeletedSwitchId="include-deleted-categories"
      buttonLabel={t("categories.actions.create")}
      onCreate={handleAddCategory}
      columns={categoryColumns}
      data={data?.items ?? []}
      emptyStateLabel={t("categories.empty")}
      loading={isLoading}
      loadingLabel={t("categories.loading")}
      error={error}
      pagination={{
        totalCount,
        page: currentPage,
        totalPages,
        totalCountLabel: t("categories.pagination.rows"),
        pageLabel: t("categories.pagination.page"),
        previousLabel: t("categories.pagination.previous"),
        nextLabel: t("categories.pagination.next"),
        onPageChange: handlePageChange,
      }}
      searchPlaceholder={t("categories.searchPlaceholder")}
    >
      <CategoryDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </EntityManagementPage>
  );
}
