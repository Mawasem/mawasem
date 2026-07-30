import { Check, MapPin } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { getCheckoutCopy } from "../i18n/checkout-copy"
import type { CustomerAddress } from "../types/checkout.types"
interface Props {
  address: CustomerAddress
  selected: boolean
  onSelect: () => void
}
export function AddressCard({ address, selected, onSelect }: Props) {
  const copy = getCheckoutCopy()

  return (
    <Card
      className={selected ? "border-primary ring-2 ring-primary/20" : undefined}
    >
      <CardContent className="flex items-start gap-3 p-4">
        <MapPin className="mt-1 size-5 shrink-0 text-primary" />
        <div className="min-w-0 flex-1 space-y-1">
          <div className="flex flex-wrap items-center gap-2">
            <h3 className="font-semibold">{address.label}</h3>
            {address.isDefault ? (
              <Badge variant="secondary">{copy.address.defaultBadge}</Badge>
            ) : null}
          </div>
          <p className="text-sm">
            {address.detailedAddress}, {address.areaName}, {address.city}
          </p>
          <p className="text-sm text-muted-foreground">
            {address.recipientName} · {address.recipientPhone}
          </p>
          <p className="text-sm text-muted-foreground">
            {copy.address.delivery}:{" "}
            {address.deliveryArea.isFreeDelivery
              ? copy.summary.free
              : `EGP ${address.deliveryArea.effectiveDeliveryFee.toFixed(2)}`}
          </p>
        </div>
        <Button
          type="button"
          size="icon"
          variant={selected ? "default" : "outline"}
          onClick={onSelect}
        >
          <Check className="size-4" />
          <span className="sr-only">{copy.address.select}</span>
        </Button>
      </CardContent>
    </Card>
  )
}
