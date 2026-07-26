import { z } from "zod"

export const createResetEmployeePasswordSchema = (t: (key: string) => string) =>
  z
    .object({
      temporaryPassword: z.string(),
      confirmTemporaryPassword: z.string(),
    })
    .superRefine((values, context) => {
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

export type ResetEmployeePasswordFormValues = z.infer<
  ReturnType<typeof createResetEmployeePasswordSchema>
>

export const resetEmployeePasswordDefaultValues: ResetEmployeePasswordFormValues =
  {
    temporaryPassword: "",
    confirmTemporaryPassword: "",
  }
