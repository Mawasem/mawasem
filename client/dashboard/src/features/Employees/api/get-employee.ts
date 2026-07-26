import { api } from "@/lib/axios"

import type { Employee } from "../types"

export async function getEmployee(employeeId: number) {
  const response = await api.get<Employee>(`/employees/${employeeId}`)

  return response.data
}
