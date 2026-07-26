import { useMemo, useState } from "react"
import { useTranslation } from "react-i18next"

import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"

import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useEmployeeAccessOptions } from "../hooks/use-employee-access-options"
import { useUpdateEmployeePermissions } from "../hooks/use-update-employee-permissions"
import type { Employee, ManageEmployeePermissionsDialogProps } from "../types"

export function ManageEmployeePermissionsDialog({
  employee,
  open,
  onOpenChange,
}: ManageEmployeePermissionsDialogProps) {
  const { t, i18n } = useTranslation()

  const employeeName =
    i18n.resolvedLanguage === "ar" ? employee.fullNameAr : employee.fullNameEn

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="flex max-h-[85vh] flex-col overflow-hidden sm:max-w-4xl">
        <DialogHeader className="shrink-0">
          <DialogTitle>{t("employees.permissionsDialog.title")}</DialogTitle>

          <DialogDescription>
            {t("employees.permissionsDialog.description", {
              name: employeeName,
            })}
          </DialogDescription>
        </DialogHeader>

        {open ? (
          <ManageEmployeePermissionsContent
            key={employee.id}
            employee={employee}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  )
}

interface ManageEmployeePermissionsContentProps {
  employee: Employee
  onClose: () => void
}

function ManageEmployeePermissionsContent({
  employee,
  onClose,
}: ManageEmployeePermissionsContentProps) {
  const { t } = useTranslation()

  const [search, setSearch] = useState("")

  const [selectedPermissions, setSelectedPermissions] = useState<string[]>(
    () => [...employee.directPermissions]
  )

  const {
    employeeAccessOptionsData,
    isLoading: isLoadingAccessOptions,
    error: accessOptionsError,
  } = useEmployeeAccessOptions({
    enabled: true,
  })

  const updatePermissionsMutation = useUpdateEmployeePermissions()

  const filteredPermissionNames = useMemo(() => {
    const permissionNames = employeeAccessOptionsData?.permissionNames ?? []

    const normalizedSearch = search.trim().toLowerCase()

    if (!normalizedSearch) {
      return permissionNames
    }

    return permissionNames.filter((permissionName) =>
      permissionName.toLowerCase().includes(normalizedSearch)
    )
  }, [employeeAccessOptionsData?.permissionNames, search])

  const assignablePermissionNames =
    employeeAccessOptionsData?.permissionNames ?? []

  const selectedAssignablePermissions = selectedPermissions.filter(
    (permissionName) => assignablePermissionNames.includes(permissionName)
  )

  const groupedPermissionNames = useMemo(() => {
    return filteredPermissionNames.reduce<Record<string, string[]>>(
      (groups, permissionName) => {
        const [groupName] = permissionName.split(".")

        const resolvedGroupName =
          groupName && groupName.length > 0
            ? groupName
            : t("employees.permissionsDialog.groupOther")

        groups[resolvedGroupName] ??= []
        groups[resolvedGroupName].push(permissionName)

        return groups
      },
      {}
    )
  }, [filteredPermissionNames, t])

  const sortedGroups = useMemo(
    () =>
      Object.entries(groupedPermissionNames).sort(([first], [second]) =>
        first.localeCompare(second)
      ),
    [groupedPermissionNames]
  )

  const errorMessage =
    getEmployeeErrorMessage(updatePermissionsMutation.error) ??
    getEmployeeErrorMessage(accessOptionsError)

  const isSaving = updatePermissionsMutation.isLoading

  const handlePermissionChange = (permissionName: string, checked: boolean) => {
    setSelectedPermissions((currentPermissions) => {
      if (checked) {
        if (currentPermissions.includes(permissionName)) {
          return currentPermissions
        }

        return [...currentPermissions, permissionName]
      }

      return currentPermissions.filter((name) => name !== permissionName)
    })
  }

  const handleGroupChange = (groupPermissions: string[], checked: boolean) => {
    setSelectedPermissions((currentPermissions) => {
      if (checked) {
        return Array.from(new Set([...currentPermissions, ...groupPermissions]))
      }

      return currentPermissions.filter(
        (permissionName) => !groupPermissions.includes(permissionName)
      )
    })
  }

  const handleSave = async () => {
    try {
      await updatePermissionsMutation.updateEmployeePermissionsAsync({
        employeeId: employee.id,
        data: {
          permissionNames: selectedAssignablePermissions,
        },
      })

      onClose()
    } catch {
      // Keep dialog open and show mutation error.
    }
  }

  return (
    <>
      <div className="space-y-4">
        <div className="space-y-2">
          <Label htmlFor="employee-permissions-search">
            {t("employees.permissionsDialog.searchLabel")}
          </Label>

          <Input
            id="employee-permissions-search"
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder={t("employees.permissionsDialog.searchPlaceholder")}
            disabled={isSaving}
          />
        </div>

        <div className="flex flex-wrap items-center justify-between gap-2">
          <p className="text-sm text-muted-foreground">
            {t("employees.permissionsDialog.selectedCount", {
              count: selectedAssignablePermissions.length,
            })}
          </p>

          <Badge variant="secondary">
            {t("employees.permissionsDialog.effectiveCount", {
              count: employee.effectivePermissions.length,
            })}
          </Badge>
        </div>
      </div>

      <div className="min-h-0 flex-1 overflow-y-auto pe-1">
        {isLoadingAccessOptions ? (
          <p className="text-sm text-muted-foreground">
            {t("employees.permissionsDialog.loading")}
          </p>
        ) : null}

        {!isLoadingAccessOptions &&
        !accessOptionsError &&
        sortedGroups.length === 0 ? (
          <p className="text-sm text-muted-foreground">
            {t("employees.permissionsDialog.empty")}
          </p>
        ) : null}

        <div className="space-y-6">
          {sortedGroups.map(([groupName, groupPermissions]) => {
            const selectedGroupCount = groupPermissions.filter(
              (permissionName) => selectedPermissions.includes(permissionName)
            ).length

            const isGroupFullySelected =
              groupPermissions.length > 0 &&
              selectedGroupCount === groupPermissions.length

            return (
              <section key={groupName} className="space-y-3">
                <div className="flex flex-wrap items-center justify-between gap-3 border-b pb-2">
                  <div className="flex items-center gap-3">
                    <Checkbox
                      id={`employee-permission-group-${groupName}`}
                      checked={isGroupFullySelected}
                      onCheckedChange={(checked) =>
                        handleGroupChange(groupPermissions, checked === true)
                      }
                      disabled={isSaving}
                    />

                    <Label
                      htmlFor={`employee-permission-group-${groupName}`}
                      className="font-semibold"
                    >
                      {groupName}
                    </Label>
                  </div>

                  <Badge variant="outline">
                    {selectedGroupCount}/{groupPermissions.length}
                  </Badge>
                </div>

                <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                  {groupPermissions.map((permissionName) => {
                    const isChecked =
                      selectedPermissions.includes(permissionName)

                    const checkboxId = `employee-permission-${employee.id}-${permissionName}`

                    return (
                      <Label
                        key={permissionName}
                        htmlFor={checkboxId}
                        className="flex cursor-pointer items-start gap-3 rounded-lg border p-3"
                      >
                        <Checkbox
                          id={checkboxId}
                          checked={isChecked}
                          onCheckedChange={(checked) =>
                            handlePermissionChange(
                              permissionName,
                              checked === true
                            )
                          }
                          disabled={isSaving}
                        />

                        <span className="min-w-0 text-sm font-medium wrap-break-word">
                          {permissionName}
                        </span>
                      </Label>
                    )
                  })}
                </div>
              </section>
            )
          })}
        </div>
      </div>

      {errorMessage ? (
        <p className="text-sm text-destructive">{errorMessage}</p>
      ) : null}

      <DialogFooter className="shrink-0">
        <Button
          type="button"
          variant="outline"
          onClick={onClose}
          disabled={isSaving}
        >
          {t("common.cancel")}
        </Button>

        <Button
          type="button"
          onClick={handleSave}
          disabled={
            isLoadingAccessOptions || isSaving || accessOptionsError !== null
          }
        >
          {isSaving
            ? t("common.saving")
            : t("employees.permissionsDialog.save")}
        </Button>
      </DialogFooter>
    </>
  )
}
