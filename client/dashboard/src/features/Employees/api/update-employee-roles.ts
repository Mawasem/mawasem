import { api } from "@/lib/axios"

import type { Employee, UpdateEmployeeRolesParams } from "../types"

export async function updateEmployeeRoles({
  employeeId,
  data,
}: UpdateEmployeeRolesParams) {
  const response = await api.put<Employee>(
    `/employees/${employeeId}/roles`,
    data
  )

  return response.data
}
