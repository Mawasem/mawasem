import type { TFunction } from "i18next"
import {
  DeliveryMethod,
  OrderSource,
  OrderStatus,
  PaymentMethod,
  PaymentStatus,
} from "./types"

function getNumericEnumKey<TValues extends Record<string, number>>(
  values: TValues,
  value: TValues[keyof TValues]
) {
  const entry = Object.entries(values).find(
    ([, candidate]) => candidate === value
  )

  return (
    entry?.[0].replace(/([a-z])([A-Z])/g, "$1_$2").toLowerCase() ?? "unknown"
  )
}

export function getOrderStatusKey(status: OrderStatus) {
  return getNumericEnumKey(OrderStatus, status)
}

export function getPaymentMethodKey(value: PaymentMethod) {
  return getNumericEnumKey(PaymentMethod, value)
}

export function getPaymentStatusKey(value: PaymentStatus) {
  return getNumericEnumKey(PaymentStatus, value)
}

export function getDeliveryMethodKey(value: DeliveryMethod) {
  return getNumericEnumKey(DeliveryMethod, value)
}

export function getOrderSourceKey(value: OrderSource) {
  return getNumericEnumKey(OrderSource, value)
}

export function formatOrderMoney(value: number, language: string) {
  return new Intl.NumberFormat(language === "ar" ? "ar-EG" : "en-GB", {
    style: "currency",
    currency: "EGP",
    minimumFractionDigits: 2,
  }).format(value)
}

export function formatOrderDate(value: string, language: string) {
  return new Intl.DateTimeFormat(language === "ar" ? "ar-EG" : "en-GB", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value))
}

export function translateOrderStatus(status: OrderStatus, t: TFunction) {
  return t(`orders.status.${getOrderStatusKey(status)}`)
}
