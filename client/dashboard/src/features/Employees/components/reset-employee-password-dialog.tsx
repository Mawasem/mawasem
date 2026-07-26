import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { useTranslation } from "react-i18next"

import { EntityDialog } from "@/components/entity-dialog/entity-dialog"
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import { Input } from "@/components/ui/input"

import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useResetEmployeePassword } from "../hooks/use-reset-employee-password"
import {
  createResetEmployeePasswordSchema,
  resetEmployeePasswordDefaultValues,
  type ResetEmployeePasswordFormValues,
} from "../schema/reset-employee-password-schema"
import type { Employee, ResetEmployeePasswordDialogProps } from "../types"

export function ResetEmployeePasswordDialog({
  employee,
  open,
  onOpenChange,
}: ResetEmployeePasswordDialogProps) {
  const { t, i18n } = useTranslation()

  const employeeName =
    i18n.resolvedLanguage === "ar" ? employee.fullNameAr : employee.fullNameEn

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={t("employees.resetPasswordDialog.title")}
      description={t("employees.resetPasswordDialog.description", {
        name: employeeName,
      })}
    >
      {open ? (
        <ResetEmployeePasswordContent
          key={employee.id}
          employee={employee}
          onClose={() => onOpenChange(false)}
        />
      ) : null}
    </EntityDialog>
  )
}

interface ResetEmployeePasswordContentProps {
  employee: Employee
  onClose: () => void
}

function ResetEmployeePasswordContent({
  employee,
  onClose,
}: ResetEmployeePasswordContentProps) {
  const { t } = useTranslation()

  const resetPasswordMutation = useResetEmployeePassword()

  const resetPasswordSchema = createResetEmployeePasswordSchema(t)

  const form = useForm<ResetEmployeePasswordFormValues>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: resetEmployeePasswordDefaultValues,
  })

  const formId = `employee-reset-password-form-${employee.id}`

  const errorMessage = getEmployeeErrorMessage(resetPasswordMutation.error)

  const handleSubmit = async (values: ResetEmployeePasswordFormValues) => {
    try {
      await resetPasswordMutation.resetEmployeePasswordAsync({
        employeeId: employee.id,
        data: {
          temporaryPassword: values.temporaryPassword,
          confirmTemporaryPassword: values.confirmTemporaryPassword,
        },
      })

      onClose()
    } catch {
      // Keep dialog open and show mutation error.
    }
  }

  return (
    <div className="space-y-5">
      <p className="text-sm text-muted-foreground">
        {t("employees.resetPasswordDialog.notice")}
      </p>

      <Form {...form}>
        <form
          id={formId}
          onSubmit={form.handleSubmit(handleSubmit)}
          className="space-y-5"
        >
          <FormField
            control={form.control}
            name="temporaryPassword"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  {t("employees.resetPasswordDialog.temporaryPasswordLabel")}
                </FormLabel>

                <FormControl>
                  <Input
                    type="password"
                    autoComplete="new-password"
                    placeholder={t(
                      "employees.resetPasswordDialog.temporaryPasswordPlaceholder"
                    )}
                    {...field}
                  />
                </FormControl>

                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="confirmTemporaryPassword"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  {t(
                    "employees.resetPasswordDialog.confirmTemporaryPasswordLabel"
                  )}
                </FormLabel>

                <FormControl>
                  <Input
                    type="password"
                    autoComplete="new-password"
                    placeholder={t(
                      "employees.resetPasswordDialog.confirmTemporaryPasswordPlaceholder"
                    )}
                    {...field}
                  />
                </FormControl>

                <FormMessage />
              </FormItem>
            )}
          />

          {errorMessage ? (
            <p className="text-sm text-destructive">{errorMessage}</p>
          ) : null}
        </form>
      </Form>

      <EntityDialogFooter
        mode="edit"
        formId={formId}
        isLoading={resetPasswordMutation.isLoading}
        onCancel={onClose}
        cancelLabel={t("common.cancel")}
        editLabel={t("employees.resetPasswordDialog.save")}
        editLoadingLabel={t("common.saving")}
      />
    </div>
  )
}
