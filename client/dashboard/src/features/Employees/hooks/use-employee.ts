import { useQuery } from "@tanstack/react-query"

import { getEmployee } from "../api/get-employee"

export function useEmployee(employeeId: number, enabled = true) {
  const {
    data: employeeData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["employee", employeeId],
    queryFn: () => getEmployee(employeeId),
    enabled: enabled && employeeId > 0,
  })

  return {
    employeeData,
    isLoading,
    error,
  }
}
