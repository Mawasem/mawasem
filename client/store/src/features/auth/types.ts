export const customerGender = { male: 1, female: 2 } as const
export type CustomerGender =
  (typeof customerGender)[keyof typeof customerGender]

export const customerReferralSource = {
  facebook: 1,
  instagram: 2,
  tiktok: 3,
  google: 4,
  friend: 5,
  other: 6,
} as const
export type CustomerReferralSource =
  (typeof customerReferralSource)[keyof typeof customerReferralSource]

export interface RegisterCustomerRequest {
  fullNameAr: string
  fullNameEn: string
  phoneNumber: string
  birthDate: string | null
  gender: CustomerGender
  referralSource: CustomerReferralSource
  password: string
  confirmPassword: string
}

export interface LoginCustomerRequest {
  phoneNumber: string
  password: string
}

export interface ForgotCustomerPasswordRequest {
  phoneNumber: string
}

export interface VerifyCustomerPasswordResetCodeRequest {
  phoneNumber: string
  code: string
}

export interface VerifyCustomerPasswordResetCodeResponse {
  resetToken: string
  expiresAtUtc: string
}

export interface ResetCustomerPasswordRequest {
  phoneNumber: string
  resetToken: string
  newPassword: string
  confirmNewPassword: string
}

export interface CustomerUser {
  id: number
  fullNameAr: string
  fullNameEn: string
  phoneNumber: string
  email: string | null
  roles: string[]
}

export interface CustomerAuthenticationResponse {
  tokenType: "Bearer" | string
  accessToken: string
  accessTokenExpiresAtUtc: string
  user: CustomerUser
}

export interface ApiProblemDetails {
  title?: string
  status?: number
  detail?: string
  code?: string
  errors?: Record<string, string[]>
}
