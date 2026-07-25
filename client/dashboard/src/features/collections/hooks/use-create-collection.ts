import { useMutation, useQueryClient } from "@tanstack/react-query";

import { createCollection } from "../api/create-collection";

export const useCreateCollection = () => {
  const queryClient = useQueryClient();

  const {
    mutate: createCollectionMutation,
    mutateAsync: createCollectionAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: createCollection,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["collections"],
      });
    },
  });

  return {
    createCollection: createCollectionMutation,
    createCollectionAsync,
    isLoading,
    error,
  };
};