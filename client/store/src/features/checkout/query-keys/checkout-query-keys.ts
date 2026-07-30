export const checkoutQueryKeys = {
  all: ["checkout"] as const,
  addresses: ["checkout", "addresses"] as const,
  deliveryAreas: ["checkout", "delivery-areas"] as const,
  preview: (
    cartId: number | null,
    deliveryMethod: number,
    addressId: number | null,
    paymentMethod: number
  ) =>
    [
      "checkout",
      "preview",
      cartId,
      deliveryMethod,
      addressId,
      paymentMethod,
    ] as const,
}
