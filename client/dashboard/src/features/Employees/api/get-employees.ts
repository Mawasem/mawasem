import { api } from "@/lib/axios"

import type { EmployeesResponse, GetEmployeesParams } from "../types"

export async function getEmployees(params: GetEmployeesParams) {
  const response = await api.get<EmployeesResponse>("/employees", {
    params,
  })

  return response.data
}
