import { useMutation, useQueryClient } from "@tanstack/react-query";

import { restoreCollection } from "../api/restore-collection";

export const useRestoreCollection = () => {
  const queryClient = useQueryClient();

  const {
    mutate: restoreCollectionMutation,
    mutateAsync: restoreCollectionAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: restoreCollection,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["collections"],
      });
    },
  });

  return {
    restoreCollection: restoreCollectionMutation,
    restoreCollectionAsync,
    isLoading,
    error,
  };
};