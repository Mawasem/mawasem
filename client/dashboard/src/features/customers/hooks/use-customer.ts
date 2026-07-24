import { useQuery } from "@tanstack/react-query";
import { getCustomer } from "../api/get-customer";
import type { CustomerDetails } from "../types";


export const useCustomer = (customerId: number) => {
  const {
    data: customerData,
    isPending: isLoading,
    error,
  } = useQuery<CustomerDetails>({
    queryKey: ["customer", customerId],
    queryFn: () => getCustomer(customerId),
    enabled: !!customerId,
  });

  return {
    customerData,
    isLoading,
    error,
  };
};