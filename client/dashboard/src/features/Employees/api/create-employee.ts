import { api } from "@/lib/axios"

import type { CreateEmployeeRequest, Employee } from "../types"

export async function createEmployee(data: CreateEmployeeRequest) {
  const response = await api.post<Employee>("/employees", data)

  return response.data
}
