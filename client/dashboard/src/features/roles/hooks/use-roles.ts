import { useQuery } from "@tanstack/react-query";

import { getRoles } from "../api/get-roles";

export function useRoles() {
  const {
    data,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["roles"],
    queryFn: getRoles,
  });

  return {
    data,
    isLoading,
    error,
  };
}
