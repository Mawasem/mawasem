import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useDebounce } from "use-debounce";

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { normalizeArabic } from "@/lib/normalize-arabic";

import { useSeasonColumns } from "../components/season-columns";
import { SeasonDialog } from "../components/season-dialog";
import { useSeasons } from "../hooks/use-seasons";

export default function SeasonsPage() {
  const { t } = useTranslation();

  const seasonColumns = useSeasonColumns();

  const [searchInput, setSearchInput] = useState("");

  const normalizedSearch =
    normalizeArabic(searchInput);

  const [debouncedSearch] = useDebounce(
    normalizedSearch,
    500
  );

  const [requestedPageNumber, setRequestedPageNumber] =
    useState(1);
  const [includeDeleted, setIncludeDeleted] =
    useState(false);
  const [showOnlyActive, setShowOnlyActive] =
    useState(false);
  const [isCreateDialogOpen, setIsCreateDialogOpen] =
    useState(false);

  const {
    data,
    isLoading,
    error,
  } = useSeasons({
    search:
      debouncedSearch.length > 0
        ? debouncedSearch
        : undefined,
    isActive: showOnlyActive ? true : undefined,
    includeDeleted,
    pageNumber: requestedPageNumber,
    pageSize: 10,
  });

  const currentPage =
    data?.pageNumber ?? requestedPageNumber;

  const totalPages = data?.totalPages ?? 1;
  const totalCount = data?.totalCount ?? 0;

  const handleSearch = (value: string) => {
    setSearchInput(value);
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

  const handleIncludeDeletedChange = (
    value: boolean
  ) => {
    setIncludeDeleted(value);
    setRequestedPageNumber(1);
  };

  const handleShowOnlyActiveChange = (
    value: boolean
  ) => {
    setShowOnlyActive(value);
    setRequestedPageNumber(1);
  };

  const handleAddSeason = () => {
    setIsCreateDialogOpen(true);
  };

  return (
    <EntityManagementPage
      title={t("seasons.page.title")}
      description={t("seasons.page.description")}
      search={searchInput}
      onSearch={handleSearch}
      includeDeleted={includeDeleted}
      onIncludeDeletedChange={handleIncludeDeletedChange}
      includeDeletedLabel={t("seasons.filters.includeDeleted")}
      includeDeletedSwitchId="include-deleted-seasons"
      buttonLabel={t("seasons.actions.create")}
      onCreate={handleAddSeason}
      columns={seasonColumns}
      data={data?.items ?? []}
      emptyStateLabel={t("seasons.empty")}
      loading={isLoading}
      loadingLabel={t("seasons.loading")}
      error={error}
      errorRenderer={(nextError) =>
        t("seasons.errors.generic", {
          message: nextError.message,
        })
      }
      pagination={{
        totalCount,
        page: currentPage,
        totalPages,
        totalCountLabel: t("seasons.pagination.rows"),
        pageLabel: t("seasons.pagination.page"),
        previousLabel: t("seasons.pagination.previous"),
        nextLabel: t("seasons.pagination.next"),
        onPageChange: handlePageChange,
      }}
      searchPlaceholder={t("seasons.searchPlaceholder")}
      filtersSlot={
        <div className="flex items-center gap-2">
          <Switch
            id="active-seasons-only"
            checked={showOnlyActive}
            onCheckedChange={
              handleShowOnlyActiveChange
            }
          />

          <Label htmlFor="active-seasons-only">
            {t("seasons.filters.activeOnly")}
          </Label>
        </div>
      }
    >
      <SeasonDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </EntityManagementPage>
  );
}
