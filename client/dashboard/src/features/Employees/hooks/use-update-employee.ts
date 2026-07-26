import { useMutation, useQueryClient } from "@tanstack/react-query"

import { updateEmployee } from "../api/update-employee"

export function useUpdateEmployee() {
  const queryClient = useQueryClient()

  const {
    mutate: updateEmployeeMutation,
    mutateAsync: updateEmployeeAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: updateEmployee,

    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["employees"],
      })

      queryClient.invalidateQueries({
        queryKey: ["employee", variables.employeeId],
      })
    },
  })

  return {
    updateEmployee: updateEmployeeMutation,
    updateEmployeeAsync,
    isLoading,
    error,
  }
}
