import { useState } from "react";
import { useDebounce } from "use-debounce";

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage";
import { normalizeArabic } from "@/lib/normalize-arabic";

import { useBrands } from "../hooks/use-brands";
import { brandColumns } from "@/components/entity-table/columns/brand-columns";
import { BrandDialog } from "../components/brand-dialog";


export function BrandsPage() {
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
  const [isCreateDialogOpen, setIsCreateDialogOpen] =
    useState(false);

  const {
    data,
  } = useBrands({
    search:
      debouncedSearch.length > 0
        ? debouncedSearch
        : undefined,
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

  const handleAddBrand = () => {
    setIsCreateDialogOpen(true);
  };

  return (
    <EntityManagementPage
      title="Brands"
      description="Manage your brands."
      search={searchInput}
      onSearch={handleSearch}
      includeDeleted={includeDeleted}
      onIncludeDeletedChange={handleIncludeDeletedChange}
      includeDeletedLabel="Include deleted"
      includeDeletedSwitchId="include-deleted-brands"
      buttonLabel="Add Brand"
      onCreate={handleAddBrand}
      columns={brandColumns}
      data={data?.items ?? []}
      loading={false}
      error={null}
      pagination={{
        totalCount,
        page: currentPage,
        totalPages,
        onPageChange: handlePageChange,
      }}
    >
      <BrandDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </EntityManagementPage>
  );
}