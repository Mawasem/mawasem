
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { useDebounce } from "use-debounce";

import { EntityPagination } from "@/components/entity-table/entity-pagination";
import { EntityTable } from "@/components/entity-table/entity-table";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { normalizeArabic } from "@/lib/normalize-arabic";

import { useCustomerColumns } from "../components/customer-columns";
import { useCustomers } from "../hooks/use-customers";

export default function CustomerPage() {
  const { t } = useTranslation();

  const customerColumns = useCustomerColumns();

  const [searchInput, setSearchInput] = useState("");

  const normalizedSearch =
    normalizeArabic(searchInput);

  const [debouncedSearch] = useDebounce(
    normalizedSearch,
    500
  );

  const [requestedPageNumber, setRequestedPageNumber] =
    useState(1);
  const [showBlockedOnly, setShowBlockedOnly] =
    useState(false);

  const {
    customersData,
    isLoading,
    error,
  } = useCustomers({
    search:
      debouncedSearch.length > 0
        ? debouncedSearch
        : undefined,
    isBlocked: showBlockedOnly ? true : undefined,
    pageNumber: requestedPageNumber,
    pageSize: 10,
  });

  const currentPage =
    customersData?.pageNumber ?? requestedPageNumber;

  const totalPages =
    customersData?.totalPages ?? 1;

  const totalCount =
    customersData?.totalCount ?? 0;

  const handleSearch = (value: string) => {
    setSearchInput(value);
    setRequestedPageNumber(1);
  };

  const handleShowBlockedOnlyChange = (
    value: boolean
  ) => {
    setShowBlockedOnly(value);
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

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">
          {t("customers.page.title")}
        </h1>

        <p className="text-muted-foreground">
          {t("customers.page.description")}
        </p>
      </div>

      <Input
        value={searchInput}
        onChange={(event) =>
          handleSearch(event.target.value)
        }
        placeholder={t("customers.searchPlaceholder")}
        className="max-w-sm"
      />

      <div className="flex items-center gap-2">
        <Switch
          id="blocked-customers-only"
          checked={showBlockedOnly}
          onCheckedChange={
            handleShowBlockedOnlyChange
          }
        />

        <Label htmlFor="blocked-customers-only">
          {t("customers.filters.blockedOnly")}
        </Label>
      </div>

      <EntityTable
        columns={customerColumns}
        data={customersData?.items ?? []}
        emptyStateLabel={t("customers.empty")}
      />

      {isLoading ? (
        <p className="text-sm text-muted-foreground">
          {t("customers.loading")}
        </p>
      ) : null}

      {error instanceof Error ? (
        <p className="text-sm text-destructive">
          {t("customers.errors.generic", {
            message: error.message,
          })}
        </p>
      ) : null}

      <EntityPagination
        totalCount={totalCount}
        page={currentPage}
        totalPages={totalPages}
        totalCountLabel={t("customers.pagination.rows")}
        pageLabel={t("customers.pagination.page")}
        previousLabel={t("customers.pagination.previous")}
        nextLabel={t("customers.pagination.next")}
        onPageChange={handlePageChange}
      />
    </div>
  );
}
