import { useQuery } from "@tanstack/react-query";

import { getCustomers } from "../api/get-customers";
import type { CustomersQuery } from "../types";

export const useCustomers = (params: CustomersQuery) => {
  const {
    data: customersData,
    isPending: isLoading,
    error,
  } = useQuery({
    queryKey: ["customers", params],
    queryFn: () => getCustomers(params),
  });

  return {
    customersData,
    isLoading,
    error,
  };
};