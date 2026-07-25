import { useQuery } from "@tanstack/react-query";

import { getCollections } from "../api/get-collections";
import type { CollectionQueryParams } from "../types";

export const useCollections = (
  params: CollectionQueryParams
) => {
  const {
    data,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["collections", params],
    queryFn: () => getCollections(params),
  });

  return {
    data,
    isLoading,
    error,
  };
};