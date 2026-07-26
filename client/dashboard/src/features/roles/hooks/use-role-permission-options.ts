import { useQuery } from "@tanstack/react-query";

import { getRolePermissionOptions } from "../api/get-role-permission-options";

export function useRolePermissionOptions() {
  const {
    data: permissionOptionsData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["roles", "permission-options"],
    queryFn: getRolePermissionOptions,
  });

  return {
    permissionOptionsData,
    isLoading,
    error,
  };
}
