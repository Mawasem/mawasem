import { useQuery } from "@tanstack/react-query"
import type { GetEmployeesParams } from "../types"
import { getEmployees } from "../api/get-employees"

export function useEmployees(params: GetEmployeesParams) {
  const {
    data: employeesData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["employees", params],
    queryFn: () => getEmployees(params),
  })

  return {
    employeesData,
    error,
    isLoading,
  }
}
