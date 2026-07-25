import { useMutation, useQueryClient } from "@tanstack/react-query";

import { updateCollection } from "../api/update-collection";

export const useUpdateCollection = () => {
  const queryClient = useQueryClient();

  const {
    mutate: updateCollectionMutation,
    mutateAsync: updateCollectionAsync,
    isPending: isLoading,
    error,
  } = useMutation({
    mutationFn: updateCollection,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["collections"],
      });
    },
  });

  return {
    updateCollection: updateCollectionMutation,
    updateCollectionAsync,
    isLoading,
    error,
  };
};