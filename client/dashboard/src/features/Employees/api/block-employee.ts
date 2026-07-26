import { api } from "@/lib/axios"

import type { BlockEmployeeParams } from "../types"

export async function blockEmployee({ employeeId, data }: BlockEmployeeParams) {
  await api.post(`/employees/${employeeId}/block`, data)
}
