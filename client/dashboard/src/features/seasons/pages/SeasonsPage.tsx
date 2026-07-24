import { useState } from "react";
import { useDebounce } from "use-debounce";

import { EntityPagination } from "@/components/entity-table/entity-pagination";
import { EntityTable } from "@/components/entity-table/entity-table";
import { EntityToolbar } from "@/components/entity-table/entity-toolbar";
import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { normalizeArabic } from "@/lib/normalize-arabic";

import { SeasonDialog } from "../components/season-dialog";
import { seasonColumns } from "../components/season-columns";
import { useSeasons } from "../hooks/use-seasons";

export default function SeasonsPage() {
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
    // isLoading,
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
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">
          Seasons
        </h1>

        <p className="text-muted-foreground">
          Manage your seasons.
        </p>
      </div>

      <EntityToolbar
        search={searchInput}
        onSearch={handleSearch}
        buttonText="Add Season"
        onAdd={handleAddSeason}
      />

      <div className="flex items-center gap-6">
        <div className="flex items-center gap-2">
          <Switch
            id="active-seasons-only"
            checked={showOnlyActive}
            onCheckedChange={
              handleShowOnlyActiveChange
            }
          />

          <Label htmlFor="active-seasons-only">
            Active only
          </Label>
        </div>

        <div className="flex items-center gap-2">
          <Switch
            id="include-deleted-seasons"
            checked={includeDeleted}
            onCheckedChange={
              handleIncludeDeletedChange
            }
          />

          <Label htmlFor="include-deleted-seasons">
            Include deleted
          </Label>
        </div>
      </div>

      <EntityTable
        columns={seasonColumns}
        data={data?.items ?? []}
      // isLoading={isLoading}
      />

      <EntityPagination
        totalCount={totalCount}
        page={currentPage}
        totalPages={totalPages}
        onPageChange={handlePageChange}
      />

      <SeasonDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </div>
  );
}
