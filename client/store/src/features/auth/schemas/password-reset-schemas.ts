import { z } from "zod"
const phoneNumber = z
  .string()
  .trim()
  .regex(/^(010|011|012|015)\d{8}$/, "Enter a valid Egyptian mobile number.")
export const forgotPasswordSchema = z.object({ phoneNumber })
export const verifyResetCodeSchema = z.object({
  phoneNumber,
  code: z
    .string()
    .trim()
    .regex(/^\d{6}$/, "Enter the 6-digit code."),
})
export const resetPasswordSchema = z
  .object({
    phoneNumber,
    resetToken: z.string().min(1),
    newPassword: z
      .string()
      .min(8)
      .regex(/[A-Z]/)
      .regex(/[a-z]/)
      .regex(/\d/)
      .regex(/[^A-Za-z0-9]/),
    confirmNewPassword: z.string(),
  })
  .refine((data) => data.newPassword === data.confirmNewPassword, {
    path: ["confirmNewPassword"],
    message: "Passwords do not match.",
  })
export type ForgotPasswordFormValues = z.infer<typeof forgotPasswordSchema>
export type VerifyResetCodeFormValues = z.infer<typeof verifyResetCodeSchema>
export type ResetPasswordFormValues = z.infer<typeof resetPasswordSchema>
