import { z } from "zod"

import { customerGender, customerReferralSource } from "../types"

const egyptianPhoneNumberRegex = /^(?:\+20|0020|0)?1[0125]\d{8}$/
const strongPasswordRegex =
  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$/

export const signupSchema = z
  .object({
    fullNameAr: z
      .string()
      .trim()
      .min(1, "Arabic full name is required")
      .max(200, "Arabic full name cannot exceed 200 characters"),
    fullNameEn: z
      .string()
      .trim()
      .min(1, "English full name is required")
      .max(200, "English full name cannot exceed 200 characters"),
    phoneNumber: z
      .string()
      .trim()
      .regex(egyptianPhoneNumberRegex, "Enter a valid Egyptian mobile number"),
    birthDate: z.string(),
    gender: z.union([
      z.literal(customerGender.male),
      z.literal(customerGender.female),
    ]),
    referralSource: z.union([
      z.literal(customerReferralSource.facebook),
      z.literal(customerReferralSource.instagram),
      z.literal(customerReferralSource.tiktok),
      z.literal(customerReferralSource.google),
      z.literal(customerReferralSource.friend),
      z.literal(customerReferralSource.other),
    ]),
    password: z
      .string()
      .regex(
        strongPasswordRegex,
        "Use at least 8 characters with uppercase, lowercase, number, and symbol"
      ),
    confirmPassword: z.string().min(1, "Please confirm your password"),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  })

export type SignupFormValues = z.infer<typeof signupSchema>
