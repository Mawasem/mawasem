import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { useTranslation } from "react-i18next"

import { Badge } from "@/components/ui/badge"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"

import {
  createEmployeeFormSchema,
  employeeFormDefaultValues,
  type EmployeeFormValues,
} from "../schema/employee-form-schema"
import type { Employee, EmployeeDialogMode } from "../types"

interface EmployeeFormProps {
  mode: EmployeeDialogMode
  employee?: Employee
  formId: string
  errorMessage: string | null
  availableRoleNames: string[]
  isLoadingRoleOptions: boolean
  roleOptionsErrorMessage: string | null
  onSubmit: (values: EmployeeFormValues) => Promise<void>
}

export function EmployeeForm({
  mode,
  employee,
  formId,
  errorMessage,
  availableRoleNames,
  isLoadingRoleOptions,
  roleOptionsErrorMessage,
  onSubmit,
}: EmployeeFormProps) {
  const { t } = useTranslation()

  const isEditMode = mode === "edit"

  const employeeFormSchema = createEmployeeFormSchema(t, !isEditMode)

  const form = useForm<EmployeeFormValues>({
    resolver: zodResolver(employeeFormSchema),
    defaultValues:
      isEditMode && employee
        ? {
            fullNameAr: employee.fullNameAr,
            fullNameEn: employee.fullNameEn,
            email: employee.email,
            temporaryPassword: "",
            confirmTemporaryPassword: "",
            roleNames: [],
          }
        : employeeFormDefaultValues,
  })

  const handleFormSubmit = async (values: EmployeeFormValues) => {
    await onSubmit(values)
  }

  return (
    <Form {...form}>
      <form
        id={formId}
        onSubmit={form.handleSubmit(handleFormSubmit)}
        className="space-y-5"
      >
        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="fullNameAr"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{t("employees.form.fullNameArLabel")}</FormLabel>

                <FormControl>
                  <Input
                    placeholder={t("employees.form.fullNameArPlaceholder")}
                    {...field}
                  />
                </FormControl>

                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="fullNameEn"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{t("employees.form.fullNameEnLabel")}</FormLabel>

                <FormControl>
                  <Input
                    placeholder={t("employees.form.fullNameEnPlaceholder")}
                    {...field}
                  />
                </FormControl>

                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <FormField
          control={form.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>{t("employees.form.emailLabel")}</FormLabel>

              <FormControl>
                <Input
                  type="email"
                  placeholder={t("employees.form.emailPlaceholder")}
                  {...field}
                />
              </FormControl>

              <FormMessage />
            </FormItem>
          )}
        />

        {!isEditMode ? (
          <div className="space-y-5">
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="temporaryPassword"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>
                      {t("employees.form.temporaryPasswordLabel")}
                    </FormLabel>

                    <FormControl>
                      <Input
                        type="password"
                        autoComplete="new-password"
                        placeholder={t(
                          "employees.form.temporaryPasswordPlaceholder"
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
                      {t("employees.form.confirmTemporaryPasswordLabel")}
                    </FormLabel>

                    <FormControl>
                      <Input
                        type="password"
                        autoComplete="new-password"
                        placeholder={t(
                          "employees.form.confirmTemporaryPasswordPlaceholder"
                        )}
                        {...field}
                      />
                    </FormControl>

                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="roleNames"
              render={({ field }) => (
                <FormItem className="space-y-3">
                  <div className="flex flex-wrap items-center justify-between gap-2">
                    <div>
                      <FormLabel>{t("employees.form.rolesLabel")}</FormLabel>

                      <p className="text-sm text-muted-foreground">
                        {t("employees.form.rolesHint")}
                      </p>
                    </div>

                    <Badge variant="secondary">
                      {t("employees.rolesDialog.selectedCount", {
                        count: field.value.length,
                      })}
                    </Badge>
                  </div>

                  {isLoadingRoleOptions ? (
                    <p className="text-sm text-muted-foreground">
                      {t("employees.rolesDialog.loading")}
                    </p>
                  ) : null}

                  {!isLoadingRoleOptions &&
                  !roleOptionsErrorMessage &&
                  availableRoleNames.length === 0 ? (
                    <p className="text-sm text-muted-foreground">
                      {t("employees.rolesDialog.empty")}
                    </p>
                  ) : null}

                  <div className="grid gap-3 sm:grid-cols-2">
                    {availableRoleNames.map((roleName) => {
                      const checkboxId = `create-employee-role-${roleName}`

                      return (
                        <Label
                          key={roleName}
                          htmlFor={checkboxId}
                          className="flex cursor-pointer items-center gap-3 rounded-lg border p-3"
                        >
                          <Checkbox
                            id={checkboxId}
                            checked={field.value.includes(roleName)}
                            onCheckedChange={(checked) => {
                              field.onChange(
                                checked === true
                                  ? [...field.value, roleName]
                                  : field.value.filter(
                                      (name) => name !== roleName
                                    )
                              )
                            }}
                          />

                          <span className="text-sm font-medium">
                            {roleName}
                          </span>
                        </Label>
                      )
                    })}
                  </div>

                  <FormMessage />

                  {roleOptionsErrorMessage ? (
                    <p className="text-sm text-destructive">
                      {roleOptionsErrorMessage}
                    </p>
                  ) : null}
                </FormItem>
              )}
            />
          </div>
        ) : null}

        {errorMessage ? (
          <p className="text-sm text-destructive">{errorMessage}</p>
        ) : null}
      </form>
    </Form>
  )
}
