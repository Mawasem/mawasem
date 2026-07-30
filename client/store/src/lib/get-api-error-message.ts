import axios from "axios"

import type { ApiProblemDetails } from "@/features/auth/types"

export function getApiErrorMessage(
  error: unknown,
  fallback = "Something went wrong. Please try again."
) {
  if (!axios.isAxiosError<ApiProblemDetails>(error)) {
    return error instanceof Error ? error.message : fallback
  }

  const problem = error.response?.data
  const validationMessage = problem?.errors
    ? Object.values(problem.errors).flat().at(0)
    : undefined

  return validationMessage ?? problem?.detail ?? problem?.title ?? fallback
}
