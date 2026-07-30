import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios"
import { useCustomerAuthStore } from "@/features/auth/store/use-customer-auth-store"
import type { CustomerAuthenticationResponse } from "@/features/auth/types"

const baseConfig = {
  baseURL: import.meta.env.VITE_API_URL,
  withCredentials: true,
  headers: { "Content-Type": "application/json" },
}

export const api = axios.create(baseConfig)
export const refreshApi = axios.create(baseConfig)

type RetryableConfig = InternalAxiosRequestConfig & { _retry?: boolean }
let refreshPromise: Promise<CustomerAuthenticationResponse> | null = null

const refreshSession = async () => {
  const response =
    await refreshApi.post<CustomerAuthenticationResponse>("/auth/refresh")
  return response.data
}

const authEndpointFragments = [
  "/auth/login",
  "/auth/register",
  "/auth/refresh",
  "/auth/logout",
  "/auth/forgot-password",
  "/auth/verify-reset-code",
  "/auth/reset-password",
]

api.interceptors.request.use((config) => {
  const accessToken = useCustomerAuthStore.getState().accessToken
  if (accessToken) config.headers.Authorization = `Bearer ${accessToken}`
  return config
})

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableConfig | undefined
    const url = originalRequest?.url ?? ""
    const isAuthEndpoint = authEndpointFragments.some((fragment) =>
      url.includes(fragment)
    )

    if (
      error.response?.status !== 401 ||
      !originalRequest ||
      originalRequest._retry ||
      isAuthEndpoint
    ) {
      return Promise.reject(error)
    }

    originalRequest._retry = true

    try {
      refreshPromise ??= refreshSession().finally(() => {
        refreshPromise = null
      })
      const session = await refreshPromise
      useCustomerAuthStore.getState().setSession(session)
      originalRequest.headers.Authorization = `Bearer ${session.accessToken}`
      return api(originalRequest)
    } catch (refreshError) {
      useCustomerAuthStore.getState().clearSession()
      return Promise.reject(refreshError)
    }
  }
)
