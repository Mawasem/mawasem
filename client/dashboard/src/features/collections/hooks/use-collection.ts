import { useQuery } from "@tanstack/react-query";

import { getCollection } from "../api/get-collection";

export const useCollection = (
  collectionId: number
) => {
  const {
    data: collectionData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["collection", collectionId],
    queryFn: () =>
      getCollection(collectionId),
    enabled: !!collectionId,
  });

  return {
    collectionData,
    isLoading,
    error,
  };
};