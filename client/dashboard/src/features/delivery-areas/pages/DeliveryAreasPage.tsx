import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useDebounce } from "use-debounce";

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage";
import { Label } from "@/components/ui/label";
import { normalizeArabic } from "@/lib/normalize-arabic";

import { useDeliveryAreaColumns } from "../components/delivery-area-columns";
import { DeliveryAreaDialog } from "../components/delivery-area-dialog";
import { getDeliveryAreaErrorMessage } from "../get-delivery-area-error-message";
import { useDeliveryAreas } from "../hooks/use-delivery-areas";
import { DeliveryAreaStatus } from "../types";

const selectClassName =
  "h-9 min-w-40 rounded-4xl border border-input bg-input/30 px-3 text-sm outline-none transition-colors focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50";

type StatusFilter = "all" | DeliveryAreaStatus;
type ActivityFilter = "all" | "active" | "inactive";

export default function DeliveryAreasPage() {
  const { t } = useTranslation();
  const deliveryAreaColumns = useDeliveryAreaColumns();

  const [searchInput, setSearchInput] = useState("");
  const [requestedPageNumber, setRequestedPageNumber] = useState(1);
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [statusFilter, setStatusFilter] = useState<StatusFilter>("all");
  const [activityFilter, setActivityFilter] =
    useState<ActivityFilter>("all");
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);

  const normalizedSearch = normalizeArabic(searchInput);
  const [debouncedSearch] = useDebounce(normalizedSearch, 500);

  const { deliveryAreasData, isLoading, error } = useDeliveryAreas({
    search: debouncedSearch.length > 0 ? debouncedSearch : undefined,
    status: statusFilter === "all" ? undefined : statusFilter,
    isActive:
      activityFilter === "all"
        ? undefined
        : activityFilter === "active",
    includeDeleted,
    pageNumber: requestedPageNumber,
    pageSize: 10,
  });

  const currentPage =
    deliveryAreasData?.pageNumber ?? requestedPageNumber;
  const totalPages = deliveryAreasData?.totalPages ?? 0;
  const totalCount = deliveryAreasData?.totalCount ?? 0;

  const resetToFirstPage = () => setRequestedPageNumber(1);

  const handleSearch = (value: string) => {
    setSearchInput(value);
    resetToFirstPage();
  };

  const handlePageChange = (nextPage: number) => {
    const safeTotalPages = totalPages > 0 ? totalPages : 1;

    if (
      nextPage < 1 ||
      nextPage > safeTotalPages ||
      nextPage === currentPage
    ) {
      return;
    }

    setRequestedPageNumber(nextPage);
  };

  const handleStatusFilterChange = (value: string) => {
    setStatusFilter(
      value === "all" ? "all" : (Number(value) as DeliveryAreaStatus)
    );
    resetToFirstPage();
  };

  const handleActivityFilterChange = (value: ActivityFilter) => {
    setActivityFilter(value);
    resetToFirstPage();
  };

  return (
    <EntityManagementPage
      title={t("deliveryAreas.page.title")}
      description={t("deliveryAreas.page.description")}
      search={searchInput}
      onSearch={handleSearch}
      searchPlaceholder={t("deliveryAreas.searchPlaceholder")}
      includeDeleted={includeDeleted}
      onIncludeDeletedChange={(value) => {
        setIncludeDeleted(value);
        resetToFirstPage();
      }}
      includeDeletedLabel={t("deliveryAreas.filters.includeDeleted")}
      includeDeletedSwitchId="include-deleted-delivery-areas"
      buttonLabel={t("deliveryAreas.actions.create")}
      onCreate={() => setIsCreateDialogOpen(true)}
      columns={deliveryAreaColumns}
      data={deliveryAreasData?.items ?? []}
      emptyStateLabel={t("deliveryAreas.empty")}
      loading={isLoading}
      loadingLabel={t("deliveryAreas.loading")}
      error={error}
      errorRenderer={(nextError) =>
        getDeliveryAreaErrorMessage(nextError, t)
      }
      pagination={{
        totalCount,
        page: currentPage,
        totalPages,
        totalCountLabel: t("deliveryAreas.pagination.rows"),
        pageLabel: t("deliveryAreas.pagination.page"),
        previousLabel: t("deliveryAreas.pagination.previous"),
        nextLabel: t("deliveryAreas.pagination.next"),
        onPageChange: handlePageChange,
      }}
      filtersSlot={
        <div className="flex flex-wrap items-end gap-4">
          <div className="space-y-2">
            <Label htmlFor="delivery-area-status-filter">
              {t("deliveryAreas.filters.status")}
            </Label>
            <select
              id="delivery-area-status-filter"
              className={selectClassName}
              value={statusFilter}
              onChange={(event) =>
                handleStatusFilterChange(event.target.value)
              }
            >
              <option value="all">
                {t("deliveryAreas.filters.allStatuses")}
              </option>
              <option value={DeliveryAreaStatus.Pending}>
                {t("deliveryAreas.status.pending")}
              </option>
              <option value={DeliveryAreaStatus.Confirmed}>
                {t("deliveryAreas.status.confirmed")}
              </option>
              <option value={DeliveryAreaStatus.Restricted}>
                {t("deliveryAreas.status.restricted")}
              </option>
            </select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="delivery-area-activity-filter">
              {t("deliveryAreas.filters.activity")}
            </Label>
            <select
              id="delivery-area-activity-filter"
              className={selectClassName}
              value={activityFilter}
              onChange={(event) =>
                handleActivityFilterChange(
                  event.target.value as ActivityFilter
                )
              }
            >
              <option value="all">
                {t("deliveryAreas.filters.allActivity")}
              </option>
              <option value="active">
                {t("deliveryAreas.activity.active")}
              </option>
              <option value="inactive">
                {t("deliveryAreas.activity.inactive")}
              </option>
            </select>
          </div>
        </div>
      }
    >
      <DeliveryAreaDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </EntityManagementPage>
  );
}
