import { useTranslation } from "react-i18next"

import { EntityDialog } from "@/components/entity-dialog/entity-dialog"
import { EntityDialogFooter } from "@/components/entity-dialog/entity-dialog-footer"

import { getEmployeeErrorMessage } from "../get-employee-error-message"
import { useCreateEmployee } from "../hooks/use-create-employee"
import { useEmployeeAccessOptions } from "../hooks/use-employee-access-options"
import { useUpdateEmployee } from "../hooks/use-update-employee"
import type { EmployeeFormValues } from "../schema/employee-form-schema"
import type {
  Employee,
  EmployeeDialogMode,
  EmployeeDialogProps,
} from "../types"
import { EmployeeForm } from "./employee-form"

export function EmployeeDialog({
  open,
  onOpenChange,
  mode,
  employee,
}: EmployeeDialogProps) {
  const { t } = useTranslation()

  const isEditMode = mode === "edit"

  const title = isEditMode
    ? t("employees.dialog.editTitle")
    : t("employees.dialog.createTitle")

  const description = isEditMode
    ? t("employees.dialog.editDescription")
    : t("employees.dialog.createDescription")

  return (
    <EntityDialog
      open={open}
      onOpenChange={onOpenChange}
      title={title}
      description={description}
    >
      {open ? (
        <EmployeeDialogContent
          key={`${mode}-${employee?.id ?? "new"}`}
          mode={mode}
          employee={employee}
          onClose={() => onOpenChange(false)}
        />
      ) : null}
    </EntityDialog>
  )
}

interface EmployeeDialogContentProps {
  mode: EmployeeDialogMode
  employee?: Employee
  onClose: () => void
}

function EmployeeDialogContent({
  mode,
  employee,
  onClose,
}: EmployeeDialogContentProps) {
  const { t } = useTranslation()

  const createEmployeeMutation = useCreateEmployee()
  const updateEmployeeMutation = useUpdateEmployee()

  const isEditMode = mode === "edit"

  const {
    employeeAccessOptionsData,
    isLoading: isLoadingAccessOptions,
    error: accessOptionsError,
  } = useEmployeeAccessOptions({
    enabled: !isEditMode,
  })

  const availableRoleNames = [
    ...(employeeAccessOptionsData?.roleNames ?? []),
  ].sort((first, second) => first.localeCompare(second))

  const formId = `employee-form-${mode}`

  const isSubmitting =
    createEmployeeMutation.isLoading || updateEmployeeMutation.isLoading

  const isLoadingRoleOptions = !isEditMode && isLoadingAccessOptions

  const mutationError =
    createEmployeeMutation.error ?? updateEmployeeMutation.error

  const errorMessage = getEmployeeErrorMessage(mutationError)

  const roleOptionsErrorMessage = isEditMode
    ? null
    : getEmployeeErrorMessage(accessOptionsError)

  const handleSubmit = async (values: EmployeeFormValues) => {
    try {
      if (isEditMode && employee) {
        await updateEmployeeMutation.updateEmployeeAsync({
          employeeId: employee.id,
          data: {
            fullNameAr: values.fullNameAr,
            fullNameEn: values.fullNameEn,
            email: values.email,
          },
        })
      } else {
        await createEmployeeMutation.createEmployeeAsync({
          fullNameAr: values.fullNameAr,
          fullNameEn: values.fullNameEn,
          email: values.email,
          temporaryPassword: values.temporaryPassword,
          confirmTemporaryPassword: values.confirmTemporaryPassword,
          roleNames: values.roleNames,
          permissionNames: [],
        })
      }

      onClose()
    } catch {
      // Keep dialog open and show mutation error.
    }
  }

  return (
    <div className="space-y-5">
      <EmployeeForm
        mode={mode}
        employee={employee}
        formId={formId}
        onSubmit={handleSubmit}
        errorMessage={errorMessage}
        availableRoleNames={availableRoleNames}
        isLoadingRoleOptions={isLoadingRoleOptions}
        roleOptionsErrorMessage={roleOptionsErrorMessage}
      />

      <EntityDialogFooter
        mode={mode}
        formId={formId}
        isLoading={isSubmitting || isLoadingRoleOptions}
        onCancel={onClose}
        createLabel={t("employees.actions.create")}
        createLoadingLabel={
          isLoadingRoleOptions
            ? t("employees.rolesDialog.loading")
            : t("employees.actions.creating")
        }
        editLabel={t("common.saveChanges")}
        editLoadingLabel={t("common.saving")}
        cancelLabel={t("common.cancel")}
      />
    </div>
  )
}
