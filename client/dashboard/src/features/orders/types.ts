export const OrderStatus = {
  Pending: 1,
  Confirmed: 2,
  Preparing: 3,
  Shipped: 4,
  Delivered: 5,
  Cancelled: 6,
  RefundRequested: 7,
  Refunded: 8,
  Rejected: 9,
  PartiallyRefunded: 10,
} as const

export type OrderStatus = (typeof OrderStatus)[keyof typeof OrderStatus]

export const PaymentMethod = {
  CashOnDelivery: 1,
  Online: 2,
} as const

export type PaymentMethod = (typeof PaymentMethod)[keyof typeof PaymentMethod]

export const PaymentStatus = {
  Pending: 1,
  Paid: 2,
  Failed: 3,
  Refunded: 4,
  PartiallyRefunded: 5,
} as const

export type PaymentStatus = (typeof PaymentStatus)[keyof typeof PaymentStatus]

export const DeliveryMethod = {
  HomeDelivery: 1,
  StorePickup: 2,
} as const

export type DeliveryMethod =
  (typeof DeliveryMethod)[keyof typeof DeliveryMethod]

export const OrderSource = {
  Website: 1,
  Store: 2,
} as const

export type OrderSource = (typeof OrderSource)[keyof typeof OrderSource]

export interface GetAdminOrdersParams {
  search?: string
  customerUserId?: number
  status?: OrderStatus
  paymentMethod?: PaymentMethod
  paymentStatus?: PaymentStatus
  deliveryMethod?: DeliveryMethod
  orderSource?: OrderSource
  deliveryAreaId?: number
  fromDateUtc?: string
  toDateUtc?: string
  pageNumber?: number
  pageSize?: number
}

export interface AdminOrderListItem {
  id: number
  orderNumber: string
  orderDate: string
  customerUserId: number
  customerNameAr: string
  customerNameEn: string
  customerPhone: string
  orderStatus: OrderStatus
  paymentMethod: PaymentMethod
  paymentStatus: PaymentStatus
  deliveryMethod: DeliveryMethod
  orderSource: OrderSource
  shippingDeliveryAreaId: number | null
  shippingDeliveryAreaNameAr: string | null
  shippingDeliveryAreaNameEn: string | null
  subTotal: number
  discount: number
  deliveryFee: number
  totalAmount: number
  distinctItemCount: number
  totalQuantity: number
  canConfirm: boolean
  canReject: boolean
  canCancel: boolean
}

export interface AdminOrdersResponse {
  items: AdminOrderListItem[]
  pageNumber: number
  pageSize: number
  totalCount: number
  totalPages: number
}

export interface AdminOrderCustomer {
  userId: number
  nameAr: string
  nameEn: string
  phone: string
}

export interface AdminOrderShipping {
  sourceAddressId: number | null
  deliveryAreaId: number | null
  deliveryAreaNameAr: string | null
  deliveryAreaNameEn: string | null
  recipientName: string | null
  recipientPhone: string | null
  city: string | null
  areaName: string | null
  detailedAddress: string | null
  buildingNumber: string | null
  floorNumber: string | null
  apartmentNumber: string | null
  landmark: string | null
}

export interface AdminOrderItem {
  id: number
  productId: number
  productVariantId: number
  productNameAr: string
  productNameEn: string
  sku: string
  variantSummaryAr: string
  variantSummaryEn: string
  unitPrice: number
  discountAmount: number
  quantity: number
  lineTotal: number
  refundedQuantity: number
}

export interface AdminOrderDetails {
  id: number
  orderNumber: string
  orderDate: string
  orderStatus: OrderStatus
  paymentMethod: PaymentMethod
  paymentStatus: PaymentStatus
  deliveryMethod: DeliveryMethod
  orderSource: OrderSource
  subTotal: number
  discount: number
  deliveryFee: number
  totalAmount: number
  couponCode: string | null
  notes: string | null
  idempotencyKey: string | null
  cancellationReason: string | null
  cancelledAtUtc: string | null
  rejectionReason: string | null
  rejectedAtUtc: string | null
  stockRestoredAtUtc: string | null
  distinctItemCount: number
  totalQuantity: number
  canConfirm: boolean
  canReject: boolean
  canCancel: boolean
  customer: AdminOrderCustomer
  shipping: AdminOrderShipping
  items: AdminOrderItem[]
}

export interface OrderWorkflowResponse {
  orderId: number
  orderNumber: string
  previousStatus: OrderStatus
  currentStatus: OrderStatus
  statusChanged: boolean
  stockRestored: boolean
  stockRestoredAtUtc: string | null
}

export interface OrderReasonRequest {
  reason: string
}

export interface OrderWorkflowParams {
  orderId: number
}

export interface OrderReasonWorkflowParams extends OrderWorkflowParams {
  data: OrderReasonRequest
}

export type OrderWorkflowAction =
  "confirm" | "prepare" | "ship" | "deliver" | "reject" | "cancel"
