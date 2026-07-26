import { api } from "@/lib/axios"

import type { Employee, UpdateEmployeePermissionsParams } from "../types"

export async function updateEmployeePermissions({
  employeeId,
  data,
}: UpdateEmployeePermissionsParams) {
  const response = await api.put<Employee>(
    `/employees/${employeeId}/permissions`,
    data
  )

  return response.data
}
