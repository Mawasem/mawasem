import { z } from "zod"
export const loginSchema = z.object({
  phoneNumber: z
    .string()
    .trim()
    .regex(/^(010|011|012|015)\d{8}$/, "Enter a valid Egyptian mobile number."),
  password: z.string().min(1, "Password is required."),
})
export type LoginFormValues = z.infer<typeof loginSchema>
