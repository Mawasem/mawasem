import { useQuery } from "@tanstack/react-query"
import { getCustomerAddresses } from "../api/get-customer-addresses"
import { checkoutQueryKeys } from "../query-keys/checkout-query-keys"
export function useCustomerAddresses() {
  const query = useQuery({
    queryKey: checkoutQueryKeys.addresses,
    queryFn: getCustomerAddresses,
  })
  return {
    addressesData: query.data,
    isLoading: query.isPending,
    error: query.error,
  }
}
