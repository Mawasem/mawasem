import { create } from "zustand"
import type { CustomerAuthenticationResponse, CustomerUser } from "../types"

export type CustomerAuthStatus =
  "checking" | "authenticated" | "unauthenticated"

interface CustomerAuthState {
  accessToken: string | null
  accessTokenExpiresAtUtc: string | null
  user: CustomerUser | null
  status: CustomerAuthStatus
  setSession: (session: CustomerAuthenticationResponse) => void
  clearSession: () => void
  setStatus: (status: CustomerAuthStatus) => void
}

export const useCustomerAuthStore = create<CustomerAuthState>((set) => ({
  accessToken: null,
  accessTokenExpiresAtUtc: null,
  user: null,
  status: "checking",
  setSession: (session) =>
    set({
      accessToken: session.accessToken,
      accessTokenExpiresAtUtc: session.accessTokenExpiresAtUtc,
      user: session.user,
      status: "authenticated",
    }),
  clearSession: () =>
    set({
      accessToken: null,
      accessTokenExpiresAtUtc: null,
      user: null,
      status: "unauthenticated",
    }),
  setStatus: (status) => set({ status }),
}))
