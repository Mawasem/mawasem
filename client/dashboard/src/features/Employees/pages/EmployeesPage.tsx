import { useState } from "react"
import { useTranslation } from "react-i18next"
import { useDebounce } from "use-debounce"

import { EntityManagementPage } from "@/components/entity-management/EntityManagementPage"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import { normalizeArabic } from "@/lib/normalize-arabic"

import { EmployeeDialog } from "../components/employee-dialog"
import { useEmployeeColumns } from "../components/employee-columns"
import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useEmployees } from "../hooks/use-employees"

export default function EmployeesPage() {
  const { t } = useTranslation()

  const employeeColumns = useEmployeeColumns()

  const [searchInput, setSearchInput] = useState("")

  const normalizedSearch = normalizeArabic(searchInput)

  const [debouncedSearch] = useDebounce(normalizedSearch, 500)

  const [requestedPageNumber, setRequestedPageNumber] = useState(1)

  const [showOnlyBlocked, setShowOnlyBlocked] = useState(false)

  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false)

  const { employeesData, isLoading, error } = useEmployees({
    search: debouncedSearch.length > 0 ? debouncedSearch : undefined,
    isBlocked: showOnlyBlocked ? true : undefined,
    pageNumber: requestedPageNumber,
    pageSize: 10,
  })

  const currentPage = employeesData?.pageNumber ?? requestedPageNumber

  const totalPages = employeesData?.totalPages ?? 1

  const totalCount = employeesData?.totalCount ?? 0

  const handleSearch = (value: string) => {
    setSearchInput(value)
    setRequestedPageNumber(1)
  }

  const handlePageChange = (nextPage: number) => {
    if (nextPage < 1 || nextPage > totalPages || nextPage === currentPage) {
      return
    }

    setRequestedPageNumber(nextPage)
  }

  const handleShowOnlyBlockedChange = (value: boolean) => {
    setShowOnlyBlocked(value)
    setRequestedPageNumber(1)
  }

  const handleAddEmployee = () => {
    setIsCreateDialogOpen(true)
  }

  return (
    <EntityManagementPage
      title={t("employees.page.title")}
      description={t("employees.page.description")}
      search={searchInput}
      onSearch={handleSearch}
      buttonLabel={t("employees.actions.create")}
      onCreate={handleAddEmployee}
      columns={employeeColumns}
      data={employeesData?.items ?? []}
      emptyStateLabel={t("employees.empty")}
      loading={isLoading}
      loadingLabel={t("employees.loading")}
      error={error}
      errorRenderer={(nextError) =>
        t("employees.errors.generic", {
          message: getEmployeeErrorMessage(nextError) ?? nextError.message,
        })
      }
      pagination={{
        totalCount,
        page: currentPage,
        totalPages,
        totalCountLabel: t("employees.pagination.rows"),
        pageLabel: t("employees.pagination.page"),
        previousLabel: t("employees.pagination.previous"),
        nextLabel: t("employees.pagination.next"),
        onPageChange: handlePageChange,
      }}
      searchPlaceholder={t("employees.searchPlaceholder")}
      filtersSlot={
        <div className="flex items-center gap-2">
          <Switch
            id="blocked-employees-only"
            checked={showOnlyBlocked}
            onCheckedChange={handleShowOnlyBlockedChange}
          />

          <Label htmlFor="blocked-employees-only">
            {t("employees.filters.blockedOnly")}
          </Label>
        </div>
      }
    >
      <EmployeeDialog
        mode="create"
        open={isCreateDialogOpen}
        onOpenChange={setIsCreateDialogOpen}
      />
    </EntityManagementPage>
  )
}
