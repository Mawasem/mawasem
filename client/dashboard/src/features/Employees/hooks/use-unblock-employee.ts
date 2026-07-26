import { useMutation, useQueryClient } from "@tanstack/react-query"

import { unblockEmployee } from "../api/unblock-employee"

export function useUnblockEmployee() {
  const queryClient = useQueryClient()

  const {
    mutate: unblockEmployeeMutation,
    mutateAsync: unblockEmployeeAsync,
    isPending: isLoading,
    error,
    reset: resetUnblockEmployee,
  } = useMutation({
    mutationFn: unblockEmployee,

    onSuccess: (_, employeeId) => {
      queryClient.invalidateQueries({
        queryKey: ["employees"],
      })

      queryClient.invalidateQueries({
        queryKey: ["employee", employeeId],
      })
    },
  })

  return {
    unblockEmployee: unblockEmployeeMutation,
    unblockEmployeeAsync,
    isLoading,
    error,
    resetUnblockEmployee,
  }
}
