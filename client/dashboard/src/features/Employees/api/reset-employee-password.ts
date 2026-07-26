import { api } from "@/lib/axios"

import type { ResetEmployeePasswordParams } from "../types"

export async function resetEmployeePassword({
  employeeId,
  data,
}: ResetEmployeePasswordParams) {
  await api.post(`/employees/${employeeId}/reset-password`, data)
}
