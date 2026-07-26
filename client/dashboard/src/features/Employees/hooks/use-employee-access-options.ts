import { useQuery } from "@tanstack/react-query"

import { getEmployeeAccessOptions } from "../api/get-employee-access-options"

interface UseEmployeeAccessOptionsOptions {
  enabled?: boolean
}

export function useEmployeeAccessOptions(
  options?: UseEmployeeAccessOptionsOptions
) {
  const {
    data: employeeAccessOptionsData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["employee-access-options"],
    queryFn: getEmployeeAccessOptions,
    enabled: options?.enabled ?? true,
  })

  return {
    employeeAccessOptionsData,
    isLoading,
    error,
  }
}
