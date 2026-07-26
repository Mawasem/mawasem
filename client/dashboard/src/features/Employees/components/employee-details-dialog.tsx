import { useTranslation } from "react-i18next"

import { Badge } from "@/components/ui/badge"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"

import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useEmployee } from "../hooks/use-employee"
import type { EmployeeDetailsDialogProps } from "../types"

export function EmployeeDetailsDialog({
  employee,
  open,
  onOpenChange,
}: EmployeeDetailsDialogProps) {
  const { t, i18n } = useTranslation()

  const { employeeData, isLoading, error } = useEmployee(employee.id, open)

  const locale = i18n.resolvedLanguage === "ar" ? "ar-EG" : "en-GB"

  const employeeName =
    i18n.resolvedLanguage === "ar" ? employee.fullNameAr : employee.fullNameEn

  const errorMessage = getEmployeeErrorMessage(error)

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[85vh] overflow-y-auto sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle>{t("employees.detailsDialog.title")}</DialogTitle>

          <DialogDescription>
            {t("employees.detailsDialog.description", {
              name: employeeName,
            })}
          </DialogDescription>
        </DialogHeader>

        {isLoading ? (
          <p className="text-sm text-muted-foreground">
            {t("employees.detailsDialog.loading")}
          </p>
        ) : null}

        {errorMessage ? (
          <p className="text-sm text-destructive">{errorMessage}</p>
        ) : null}

        {!isLoading && !error && employeeData ? (
          <div className="grid gap-4 sm:grid-cols-2">
            <DetailItem
              label={t("employees.detailsDialog.fields.fullNameAr")}
              value={employeeData.fullNameAr}
            />

            <DetailItem
              label={t("employees.detailsDialog.fields.fullNameEn")}
              value={employeeData.fullNameEn}
            />

            <DetailItem
              label={t("employees.detailsDialog.fields.email")}
              value={employeeData.email}
            />

            <DetailItem
              label={t("employees.detailsDialog.fields.roles")}
              value={
                employeeData.roles.length > 0
                  ? employeeData.roles.join(", ")
                  : t("common.notAvailable")
              }
            />

            <DetailItem
              label={t("employees.detailsDialog.fields.directPermissions")}
              value={t("employees.detailsDialog.countLabel", {
                count: employeeData.directPermissions.length,
              })}
            />

            <DetailItem
              label={t("employees.detailsDialog.fields.effectivePermissions")}
              value={t("employees.detailsDialog.countLabel", {
                count: employeeData.effectivePermissions.length,
              })}
            />

            <div className="space-y-2 rounded-lg border p-3">
              <p className="text-sm text-muted-foreground">
                {t("employees.detailsDialog.fields.status")}
              </p>

              <Badge
                variant={employeeData.isBlocked ? "destructive" : "default"}
              >
                {employeeData.isBlocked
                  ? t("employees.status.blocked")
                  : t("employees.status.active")}
              </Badge>
            </div>

            <div className="space-y-2 rounded-lg border p-3">
              <p className="text-sm text-muted-foreground">
                {t("employees.detailsDialog.fields.passwordStatus")}
              </p>

              <Badge variant="outline">
                {employeeData.mustChangePassword
                  ? t("employees.passwordStatus.changeRequired")
                  : t("employees.passwordStatus.ready")}
              </Badge>
            </div>

            {employeeData.isBlocked ? (
              <>
                <DetailItem
                  label={t("employees.detailsDialog.fields.blockedAt")}
                  value={
                    employeeData.blockedAt
                      ? new Intl.DateTimeFormat(locale, {
                          dateStyle: "medium",
                          timeStyle: "short",
                        }).format(new Date(employeeData.blockedAt))
                      : t("common.notAvailable")
                  }
                />

                <DetailItem
                  label={t("employees.detailsDialog.fields.blockedReason")}
                  value={employeeData.blockedReason ?? t("common.notAvailable")}
                />
              </>
            ) : null}
          </div>
        ) : null}
      </DialogContent>
    </Dialog>
  )
}

interface DetailItemProps {
  label: string
  value: string
}

function DetailItem({ label, value }: DetailItemProps) {
  return (
    <div className="space-y-1 rounded-lg border p-3">
      <p className="text-sm text-muted-foreground">{label}</p>

      <p className="font-medium wrap-break-word">{value}</p>
    </div>
  )
}
