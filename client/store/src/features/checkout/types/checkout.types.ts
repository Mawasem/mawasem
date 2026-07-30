export const PaymentMethod = { CashOnDelivery: 1, Online: 2 } as const
export type PaymentMethodValue =
  (typeof PaymentMethod)[keyof typeof PaymentMethod]

export const DeliveryMethod = {
  HomeDelivery: 1,
  StorePickup: 2,
} as const
export type DeliveryMethodValue =
  (typeof DeliveryMethod)[keyof typeof DeliveryMethod]

export interface DeliveryAreaOption {
  id: number
  nameAr: string
  nameEn: string
  deliveryFee: number
  isFreeDelivery: boolean
}
export interface DeliveryAreaListResponse {
  items: DeliveryAreaOption[]
}

export interface AddressDeliveryArea extends DeliveryAreaOption {
  status: number
  effectiveDeliveryFee: number
  isActive: boolean
}
export interface CustomerAddress {
  id: number
  label: string
  city: string
  areaName: string
  detailedAddress: string
  buildingNumber: string | null
  floorNumber: string | null
  apartmentNumber: string | null
  landmark: string | null
  recipientName: string
  recipientPhone: string
  isDefault: boolean
  isActive: boolean
  deliveryArea: AddressDeliveryArea
  createdOn: string
  lastModifiedOn: string | null
}
export interface CustomerAddressListResponse {
  items: CustomerAddress[]
}
export interface CreateCustomerAddressRequest {
  label: string
  city: string
  areaName: string
  detailedAddress: string
  buildingNumber?: string | null
  floorNumber?: string | null
  apartmentNumber?: string | null
  landmark?: string | null
  recipientName: string
  recipientPhone: string
  deliveryAreaId: number
  customDeliveryAreaNameAr?: string | null
  customDeliveryAreaNameEn?: string | null
  isDefault: boolean
}
export interface CheckoutPreviewRequest {
  userAddressId: number | null
  deliveryMethod: DeliveryMethodValue
  paymentMethod: PaymentMethodValue
}
export interface CheckoutItem {
  cartItemId: number
  productId: number
  productVariantId: number
  productNameAr: string
  productNameEn: string
  sku: string
  variantSummaryAr: string
  variantSummaryEn: string
  unitPrice: number
  quantity: number
  lineTotal: number
}
export interface CheckoutWarning {
  code: string
  message: string
}
export interface CheckoutPreview {
  cartId: number
  userAddressId: number | null
  deliveryAreaId: number | null
  items: CheckoutItem[]
  subTotal: number
  discount: number
  deliveryFee: number
  totalAmount: number
  paymentMethod: PaymentMethodValue
  deliveryMethod: DeliveryMethodValue
  canPlaceOrder: boolean
  warnings: CheckoutWarning[]
}
export interface PlaceOrderRequest extends CheckoutPreviewRequest {
  notes?: string | null
  idempotencyKey: string
}
export interface PlaceOrderResponse {
  orderId: number
  orderNumber: string
  orderDate: string
  orderStatus: number
  paymentStatus: number
  paymentMethod: PaymentMethodValue
  deliveryMethod: DeliveryMethodValue
  subTotal: number
  discount: number
  deliveryFee: number
  totalAmount: number
  isIdempotentReplay: boolean
}
