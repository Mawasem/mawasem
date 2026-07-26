import { useMutation, useQueryClient } from "@tanstack/react-query"

import { createEmployee } from "../api/create-employee"

export function useCreateEmployee() {
  const queryClient = useQueryClient()

  const {
    mutate: createEmployeeMutation,
    mutateAsync: createEmployeeAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: createEmployee,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["employees"],
      })
    },
  })

  return {
    createEmployee: createEmployeeMutation,
    createEmployeeAsync,
    isLoading,
    error,
  }
}
