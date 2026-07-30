import { MapPin, Store } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"

import {
  getCheckoutCopy,
  getCheckoutLocale,
} from "../i18n/checkout-copy"
import {
  DeliveryMethod,
  type CheckoutPreview,
  type CustomerAddress,
} from "../types/checkout.types"

interface CheckoutSummaryProps {
  preview: CheckoutPreview
  selectedAddress: CustomerAddress | null
}

export function CheckoutSummary({
  preview,
  selectedAddress,
}: CheckoutSummaryProps) {
  const copy = getCheckoutCopy()
  const locale = getCheckoutLocale()
  const isStorePickup =
    preview.deliveryMethod === DeliveryMethod.StorePickup

  return (
    <Card className="h-fit lg:sticky lg:top-24">
      <CardHeader>
        <CardTitle>{copy.summary.title}</CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        {isStorePickup ? (
          <div className="flex items-start gap-3 rounded-lg border bg-muted/40 p-3">
            <Store className="mt-0.5 size-4 shrink-0 text-primary" />
            <div className="space-y-1">
              <Badge variant="secondary">{copy.summary.pickup}</Badge>
              <p className="text-sm text-muted-foreground">
                {copy.summary.pickupDescription}
              </p>
            </div>
          </div>
        ) : selectedAddress ? (
          <div className="flex items-start gap-3 rounded-lg border bg-muted/40 p-3">
            <MapPin className="mt-0.5 size-4 shrink-0 text-primary" />
            <div className="min-w-0 space-y-1 text-sm">
              <p className="font-medium">{copy.address.selectedAddress}</p>
              <p className="text-muted-foreground">
                {selectedAddress.detailedAddress}, {selectedAddress.areaName},{" "}
                {selectedAddress.city}
              </p>
            </div>
          </div>
        ) : null}

        {preview.items.map((item) => (
          <div
            key={item.cartItemId}
            className="flex justify-between gap-4 text-sm"
          >
            <div>
              <p className="font-medium">
                {locale === "ar"
                  ? item.productNameAr || item.productNameEn
                  : item.productNameEn || item.productNameAr}{" "}
                × {item.quantity}
              </p>
              <p className="text-muted-foreground">
                {locale === "ar"
                  ? item.variantSummaryAr || item.variantSummaryEn
                  : item.variantSummaryEn || item.variantSummaryAr}
              </p>
            </div>
            <span>{formatMoney(item.lineTotal)}</span>
          </div>
        ))}

        <Separator />

        <div className="space-y-2 text-sm">
          <SummaryRow
            label={copy.summary.subtotal}
            value={formatMoney(preview.subTotal)}
          />
          <SummaryRow
            label={copy.summary.discount}
            value={`- ${formatMoney(preview.discount)}`}
          />
          <SummaryRow
            label={copy.summary.delivery}
            value={
              preview.deliveryFee === 0
                ? copy.summary.free
                : formatMoney(preview.deliveryFee)
            }
          />
        </div>

        <Separator />

        <div className="flex justify-between text-lg font-bold">
          <span>{copy.summary.total}</span>
          <span>{formatMoney(preview.totalAmount)}</span>
        </div>

        {preview.warnings.map((warning) => (
          <p key={warning.code} className="text-sm text-destructive">
            {warning.message}
          </p>
        ))}
      </CardContent>
    </Card>
  )
}

function SummaryRow({
  label,
  value,
}: {
  label: string
  value: string
}) {
  return (
    <div className="flex justify-between gap-3">
      <span>{label}</span>
      <span className="text-end">{value}</span>
    </div>
  )
}

function formatMoney(value: number) {
  return `EGP ${value.toFixed(2)}`
}
