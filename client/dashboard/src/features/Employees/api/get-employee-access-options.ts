import { api } from "@/lib/axios"

import type { EmployeeAccessOptions } from "../types"

export async function getEmployeeAccessOptions() {
  const response = await api.get<EmployeeAccessOptions>(
    "/employees/access-options"
  )

  return response.data
}
