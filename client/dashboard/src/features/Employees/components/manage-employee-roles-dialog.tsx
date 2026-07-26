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
import { Label } from "@/components/ui/label"

import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useEmployeeAccessOptions } from "../hooks/use-employee-access-options"
import { useUpdateEmployeeRoles } from "../hooks/use-update-employee-roles"
import type { Employee, ManageEmployeeRolesDialogProps } from "../types"

export function ManageEmployeeRolesDialog({
  employee,
  open,
  onOpenChange,
}: ManageEmployeeRolesDialogProps) {
  const { t, i18n } = useTranslation()

  const employeeName =
    i18n.resolvedLanguage === "ar" ? employee.fullNameAr : employee.fullNameEn

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="flex max-h-[85vh] flex-col overflow-hidden sm:max-w-3xl">
        <DialogHeader className="shrink-0">
          <DialogTitle>{t("employees.rolesDialog.title")}</DialogTitle>

          <DialogDescription>
            {t("employees.rolesDialog.description", {
              name: employeeName,
            })}
          </DialogDescription>
        </DialogHeader>

        {open ? (
          <ManageEmployeeRolesContent
            key={employee.id}
            employee={employee}
            onClose={() => onOpenChange(false)}
          />
        ) : null}
      </DialogContent>
    </Dialog>
  )
}

interface ManageEmployeeRolesContentProps {
  employee: Employee
  onClose: () => void
}

function ManageEmployeeRolesContent({
  employee,
  onClose,
}: ManageEmployeeRolesContentProps) {
  const { t } = useTranslation()

  const [selectedRoles, setSelectedRoles] = useState<string[]>(() => [
    ...employee.roles,
  ])

  const {
    employeeAccessOptionsData,
    isLoading: isLoadingAccessOptions,
    error: accessOptionsError,
  } = useEmployeeAccessOptions({
    enabled: true,
  })

  const updateRolesMutation = useUpdateEmployeeRoles()

  const sortedRoleNames = useMemo(
    () =>
      [...(employeeAccessOptionsData?.roleNames ?? [])].sort((first, second) =>
        first.localeCompare(second)
      ),
    [employeeAccessOptionsData?.roleNames]
  )

  const errorMessage =
    getEmployeeErrorMessage(updateRolesMutation.error) ??
    getEmployeeErrorMessage(accessOptionsError)

  const isSaving = updateRolesMutation.isLoading

  const handleRoleToggle = (roleName: string, checked: boolean) => {
    setSelectedRoles((currentRoles) => {
      if (checked) {
        if (currentRoles.includes(roleName)) {
          return currentRoles
        }

        return [...currentRoles, roleName]
      }

      return currentRoles.filter((name) => name !== roleName)
    })
  }

  const handleSave = async () => {
    try {
      await updateRolesMutation.updateEmployeeRolesAsync({
        employeeId: employee.id,
        data: {
          roleNames: selectedRoles,
        },
      })

      onClose()
    } catch {
      // Keep dialog open and show mutation error.
    }
  }

  return (
    <>
      <div className="flex flex-wrap items-center justify-between gap-2">
        <p className="text-sm text-muted-foreground">
          {t("employees.rolesDialog.selectedCount", {
            count: selectedRoles.length,
          })}
        </p>

        <Badge variant="secondary">
          {t("employees.rolesDialog.availableCount", {
            count: sortedRoleNames.length,
          })}
        </Badge>
      </div>

      <div className="min-h-0 flex-1 overflow-y-auto pe-1">
        {isLoadingAccessOptions ? (
          <p className="text-sm text-muted-foreground">
            {t("employees.rolesDialog.loading")}
          </p>
        ) : null}

        {!isLoadingAccessOptions &&
        !accessOptionsError &&
        sortedRoleNames.length === 0 ? (
          <p className="text-sm text-muted-foreground">
            {t("employees.rolesDialog.empty")}
          </p>
        ) : null}

        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
          {sortedRoleNames.map((roleName) => {
            const checkboxId = `employee-role-${employee.id}-${roleName}`

            return (
              <Label
                key={roleName}
                htmlFor={checkboxId}
                className="flex cursor-pointer items-center gap-3 rounded-lg border p-3"
              >
                <Checkbox
                  id={checkboxId}
                  checked={selectedRoles.includes(roleName)}
                  onCheckedChange={(checked) =>
                    handleRoleToggle(roleName, checked === true)
                  }
                  disabled={isSaving}
                />

                <span className="text-sm font-medium">{roleName}</span>
              </Label>
            )
          })}
        </div>
      </div>

      {errorMessage ? (
        <p className="text-sm text-destructive">{errorMessage}</p>
      ) : null}

      {selectedRoles.length === 0 ? (
        <p className="text-sm text-destructive">
          {t("employees.validation.roleRequired")}
        </p>
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
            isLoadingAccessOptions ||
            isSaving ||
            selectedRoles.length === 0 ||
            accessOptionsError !== null
          }
        >
          {isSaving ? t("common.saving") : t("employees.rolesDialog.save")}
        </Button>
      </DialogFooter>
    </>
  )
}
