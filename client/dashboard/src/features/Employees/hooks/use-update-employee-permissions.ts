import { useMutation, useQueryClient } from "@tanstack/react-query"

import { updateEmployeePermissions } from "../api/update-employee-permissions"

export function useUpdateEmployeePermissions() {
  const queryClient = useQueryClient()

  const {
    mutate: updateEmployeePermissionsMutation,
    mutateAsync: updateEmployeePermissionsAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: updateEmployeePermissions,

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
    updateEmployeePermissions: updateEmployeePermissionsMutation,
    updateEmployeePermissionsAsync,
    isLoading,
    error,
  }
}
