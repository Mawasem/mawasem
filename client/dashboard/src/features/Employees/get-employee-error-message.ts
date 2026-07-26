import axios from "axios"

export function getEmployeeErrorMessage(error: unknown) {
  if (axios.isAxiosError<{ detail?: string }>(error)) {
    return error.response?.data?.detail ?? error.message
  }

  return error instanceof Error ? error.message : null
}
