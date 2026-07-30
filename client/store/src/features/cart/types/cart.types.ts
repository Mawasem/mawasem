export interface CartWarning {
  code: string
  message: string
}

export interface CartItem {
  cartItemId: number
  productVariantId: number
  productId: number
  productNameEn: string
  productNameAr: string
  sku: string
  optionCombinationKey: string
  quantity: number
  unitPriceSnapshot: number
  currentUnitPrice: number
  lineTotal: number
  stockQuantity: number
  isPurchasable: boolean
  warnings: CartWarning[]
}

export interface CartDetails {
  cartId: number
  isGuest: boolean
  guestExpiresOn: string | null
  distinctItemCount: number
  totalQuantity: number
  subtotal: number
  hasWarnings: boolean
  items: CartItem[]
}

export interface GuestCartCreationResponse {
  id: number
  token: string
  expiresOn: string
}

export interface AddCartItemRequest {
  productVariantId: number
  quantity: number
}

export interface AddGuestCartItemRequest extends AddCartItemRequest {
  token: string
}

export interface UpdateCartItemRequest {
  cartItemId: number
  quantity: number
}

export interface CartMutationResponse {
  cartId: number
  affectedItemCount: number
}

export interface CartMergeResponse {
  cartId: number
  mergedItemCount: number
}
