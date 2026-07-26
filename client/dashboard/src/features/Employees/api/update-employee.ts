import { api } from "@/lib/axios"

import type { Employee, UpdateEmployeeParams } from "../types"

export async function updateEmployee({
  employeeId,
  data,
}: UpdateEmployeeParams) {
  const response = await api.put<Employee>(`/employees/${employeeId}`, data)

  return response.data
}
