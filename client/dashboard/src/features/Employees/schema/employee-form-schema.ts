import { z } from "zod"

export const createEmployeeFormSchema = (
  t: (key: string) => string,
  requirePassword: boolean
) =>
  z
    .object({
      fullNameAr: z
        .string()
        .trim()
        .min(2, t("employees.validation.fullNameArMin")),
      fullNameEn: z
        .string()
        .trim()
        .min(2, t("employees.validation.fullNameEnMin")),
      email: z.string().trim().email(t("employees.validation.emailInvalid")),
      temporaryPassword: z.string(),
      confirmTemporaryPassword: z.string(),
      roleNames: z.array(z.string()),
    })
    .superRefine((values, context) => {
      if (!requirePassword) {
        return
      }

      if (values.roleNames.length === 0) {
        context.addIssue({
          path: ["roleNames"],
          code: z.ZodIssueCode.custom,
          message: t("employees.validation.roleRequired"),
        })
      }

      if (values.temporaryPassword.trim().length === 0) {
        context.addIssue({
          path: ["temporaryPassword"],
          code: z.ZodIssueCode.custom,
          message: t("employees.validation.temporaryPasswordRequired"),
        })
      }

      if (values.confirmTemporaryPassword.trim().length === 0) {
        context.addIssue({
          path: ["confirmTemporaryPassword"],
          code: z.ZodIssueCode.custom,
          message: t("employees.validation.confirmTemporaryPasswordRequired"),
        })
      }

      if (
        values.temporaryPassword.length > 0 &&
        values.confirmTemporaryPassword.length > 0 &&
        values.temporaryPassword !== values.confirmTemporaryPassword
      ) {
        context.addIssue({
          path: ["confirmTemporaryPassword"],
          code: z.ZodIssueCode.custom,
          message: t("employees.validation.passwordsMustMatch"),
        })
      }
    })

export type EmployeeFormValues = z.infer<
  ReturnType<typeof createEmployeeFormSchema>
>

export const employeeFormDefaultValues: EmployeeFormValues = {
  fullNameAr: "",
  fullNameEn: "",
  email: "",
  temporaryPassword: "",
  confirmTemporaryPassword: "",
  roleNames: [],
}
