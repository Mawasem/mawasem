import { useMutation, useQueryClient } from "@tanstack/react-query"

import { resetEmployeePassword } from "../api/reset-employee-password"

export function useResetEmployeePassword() {
  const queryClient = useQueryClient()

  const {
    mutate: resetEmployeePasswordMutation,
    mutateAsync: resetEmployeePasswordAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: resetEmployeePassword,

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
    resetEmployeePassword: resetEmployeePasswordMutation,
    resetEmployeePasswordAsync,
    isLoading,
    error,
  }
}
