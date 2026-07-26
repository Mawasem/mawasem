import { useState } from "react"
import { useTranslation } from "react-i18next"

import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"

import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useBlockEmployee } from "../hooks/use-block-employee"
import type { BlockEmployeeDialogProps } from "../types"

export function BlockEmployeeDialog({
  employee,
  open,
  onOpenChange,
}: BlockEmployeeDialogProps) {
  const { t, i18n } = useTranslation()

  const [reason, setReason] = useState("")

  const blockEmployeeMutation = useBlockEmployee()

  const employeeName =
    i18n.resolvedLanguage === "ar" ? employee.fullNameAr : employee.fullNameEn

  const trimmedReason = reason.trim()

  const errorMessage = getEmployeeErrorMessage(blockEmployeeMutation.error)

  const handleOpenChange = (nextOpen: boolean) => {
    if (blockEmployeeMutation.isLoading) {
      return
    }

    if (!nextOpen) {
      setReason("")
      blockEmployeeMutation.resetBlockEmployee()
    }

    onOpenChange(nextOpen)
  }

  const handleBlock = async () => {
    if (!trimmedReason) {
      return
    }

    try {
      await blockEmployeeMutation.blockEmployeeAsync({
        employeeId: employee.id,
        data: {
          reason: trimmedReason,
        },
      })

      setReason("")
      onOpenChange(false)
    } catch {
      // Keep the dialog open and display the mutation error.
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>{t("employees.blockDialog.title")}</DialogTitle>

          <DialogDescription>
            {t("employees.blockDialog.description", {
              name: employeeName,
            })}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-2">
          <Label htmlFor={`block-employee-reason-${employee.id}`}>
            {t("employees.blockDialog.reasonLabel")}
          </Label>

          <Textarea
            id={`block-employee-reason-${employee.id}`}
            value={reason}
            onChange={(event) => setReason(event.target.value)}
            placeholder={t("employees.blockDialog.reasonPlaceholder")}
            disabled={blockEmployeeMutation.isLoading}
            rows={4}
          />

          {!trimmedReason && reason.length > 0 ? (
            <p className="text-sm text-destructive">
              {t("employees.blockDialog.reasonRequired")}
            </p>
          ) : null}

          {errorMessage ? (
            <p className="text-sm text-destructive">{errorMessage}</p>
          ) : null}
        </div>

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => handleOpenChange(false)}
            disabled={blockEmployeeMutation.isLoading}
          >
            {t("common.cancel")}
          </Button>

          <Button
            type="button"
            variant="destructive"
            onClick={handleBlock}
            disabled={!trimmedReason || blockEmployeeMutation.isLoading}
          >
            {blockEmployeeMutation.isLoading
              ? t("employees.actions.blocking")
              : t("employees.actions.block")}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
