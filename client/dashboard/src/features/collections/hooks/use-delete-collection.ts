import { useMutation, useQueryClient } from "@tanstack/react-query";

import { deleteCollection } from "../api/delete-collection";

export const useDeleteCollection = () => {
  const queryClient = useQueryClient();

  const {
    mutate: deleteCollectionMutation,
    mutateAsync: deleteCollectionAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: deleteCollection,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["collections"],
      });
    },
  });

  return {
    deleteCollection: deleteCollectionMutation,
    deleteCollectionAsync,
    isLoading,
    error,
  };
};