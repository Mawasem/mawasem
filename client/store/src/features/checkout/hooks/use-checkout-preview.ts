import { useQuery } from "@tanstack/react-query"
import { getCheckoutPreview } from "../api/get-checkout-preview"
import { checkoutQueryKeys } from "../query-keys/checkout-query-keys"
import {
  DeliveryMethod,
  type DeliveryMethodValue,
  type PaymentMethodValue,
} from "../types/checkout.types"

export function useCheckoutPreview(
  cartId: number | null,
  deliveryMethod: DeliveryMethodValue,
  addressId: number | null,
  paymentMethod: PaymentMethodValue
) {
  const hasRequiredAddress =
    deliveryMethod === DeliveryMethod.StorePickup ||
    (addressId !== null && addressId > 0)

  const query = useQuery({
    queryKey: checkoutQueryKeys.preview(
      cartId,
      deliveryMethod,
      addressId,
      paymentMethod
    ),
    queryFn: () =>
      getCheckoutPreview({
        userAddressId:
          deliveryMethod === DeliveryMethod.HomeDelivery ? addressId : null,
        deliveryMethod,
        paymentMethod,
      }),
    enabled: cartId !== null && cartId > 0 && hasRequiredAddress,
    retry: false,
  })
  return {
    previewData: query.data,
    isLoading: query.isPending && query.fetchStatus !== "idle",
    error: query.error,
  }
}
