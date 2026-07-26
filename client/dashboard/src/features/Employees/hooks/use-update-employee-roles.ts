import { useMutation, useQueryClient } from "@tanstack/react-query"

import { updateEmployeeRoles } from "../api/update-employee-roles"

export function useUpdateEmployeeRoles() {
  const queryClient = useQueryClient()

  const {
    mutate: updateEmployeeRolesMutation,
    mutateAsync: updateEmployeeRolesAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: updateEmployeeRoles,

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
    updateEmployeeRoles: updateEmployeeRolesMutation,
    updateEmployeeRolesAsync,
    isLoading,
    error,
  }
}
