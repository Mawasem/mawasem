import { useMutation, useQueryClient } from "@tanstack/react-query";

import { updateRolePermissions } from "../api/update-role-permissions";

export function useUpdateRolePermissions() {
  const queryClient = useQueryClient();

  const {
    mutateAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: updateRolePermissions,
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["roles"] });
    },
  });

  return {
    mutateAsync,
    isLoading,
    error,
  };
}
