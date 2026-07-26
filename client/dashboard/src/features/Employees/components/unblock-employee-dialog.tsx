import { useTranslation } from "react-i18next"

import { DeleteEntityDialog } from "@/components/entity-dialog/delete-entity-dialog"

import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useUnblockEmployee } from "../hooks/use-unblock-employee"
import type { UnblockEmployeeDialogProps } from "../types"

export function UnblockEmployeeDialog({
  employee,
  open,
  onOpenChange,
}: UnblockEmployeeDialogProps) {
  const { t, i18n } = useTranslation()

  const unblockEmployeeMutation = useUnblockEmployee()

  const errorMessage = getEmployeeErrorMessage(unblockEmployeeMutation.error)

  const handleUnblock = async () => {
    try {
      await unblockEmployeeMutation.unblockEmployeeAsync(employee.id)

      onOpenChange(false)
    } catch {
      // Keep dialog open and show mutation error.
    }
  }

  const handleOpenChange = (nextOpen: boolean) => {
    if (unblockEmployeeMutation.isLoading) {
      return
    }

    if (!nextOpen) {
      unblockEmployeeMutation.resetUnblockEmployee()
    }

    onOpenChange(nextOpen)
  }

  return (
    <DeleteEntityDialog
      open={open}
      onOpenChange={handleOpenChange}
      title={t("employees.unblockDialog.title")}
      description={t("employees.unblockDialog.description")}
      entityName={
        i18n.resolvedLanguage === "ar"
          ? employee.fullNameAr
          : employee.fullNameEn
      }
      isDeleting={unblockEmployeeMutation.isLoading}
      errorMessage={errorMessage}
      confirmLabel={t("employees.actions.unblock")}
      deletingLabel={t("employees.actions.unblocking")}
      cancelLabel={t("common.cancel")}
      onConfirm={handleUnblock}
    />
  )
}
