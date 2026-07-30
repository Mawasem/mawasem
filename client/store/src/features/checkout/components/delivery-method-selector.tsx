import { Store, Truck } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import {
  RadioGroup,
  RadioGroupItem,
} from "@/components/ui/radio-group"
import { cn } from "@/lib/utils"

import { getCheckoutCopy } from "../i18n/checkout-copy"
import {
  DeliveryMethod,
  type DeliveryMethodValue,
} from "../types/checkout.types"

interface DeliveryMethodSelectorProps {
  value: DeliveryMethodValue
  onValueChange: (value: DeliveryMethodValue) => void
  disabled?: boolean
}

const options = [
  {
    value: DeliveryMethod.HomeDelivery,
    icon: Truck,
    titleKey: "homeDelivery",
    descriptionKey: "homeDeliveryDescription",
    feeKey: "homeDeliveryFee",
  },
  {
    value: DeliveryMethod.StorePickup,
    icon: Store,
    titleKey: "storePickup",
    descriptionKey: "storePickupDescription",
    feeKey: "storePickupFee",
  },
] as const

export function DeliveryMethodSelector({
  value,
  onValueChange,
  disabled = false,
}: DeliveryMethodSelectorProps) {
  const copy = getCheckoutCopy().deliveryMethod

  return (
    <div className="space-y-3">
      <Label className="text-base">{copy.title}</Label>
      <RadioGroup
        value={String(value)}
        onValueChange={(nextValue) =>
          onValueChange(Number(nextValue) as DeliveryMethodValue)
        }
        disabled={disabled}
        aria-label={copy.label}
        className="grid gap-3 sm:grid-cols-2"
      >
        {options.map((option) => {
          const selected = value === option.value
          const Icon = option.icon
          const id = `delivery-method-${option.value}`

          return (
            <Label
              key={option.value}
              htmlFor={id}
              className={cn(
                "block h-full rounded-xl",
                disabled ? "cursor-not-allowed opacity-60" : "cursor-pointer"
              )}
            >
              <Card
                className={cn(
                  "h-full gap-0 py-0 transition-colors",
                  selected
                    ? "border-primary bg-primary/5 ring-2 ring-primary/20"
                    : "hover:border-primary/50"
                )}
              >
                <CardContent className="flex h-full items-start gap-4 p-4">
                  <div className="rounded-lg bg-primary/10 p-2 text-primary">
                    <Icon className="size-5" />
                  </div>
                  <div className="min-w-0 flex-1 space-y-1">
                    <div className="flex flex-wrap items-center gap-2">
                      <span className="font-semibold">
                        {copy[option.titleKey]}
                      </span>
                      {selected ? (
                        <Badge variant="secondary">{copy.selected}</Badge>
                      ) : null}
                    </div>
                    <p className="text-sm text-muted-foreground">
                      {copy[option.descriptionKey]}
                    </p>
                    <p className="text-sm font-medium">
                      {copy[option.feeKey]}
                    </p>
                  </div>
                  <RadioGroupItem
                    id={id}
                    value={String(option.value)}
                    disabled={disabled}
                    aria-label={copy[option.titleKey]}
                  />
                </CardContent>
              </Card>
            </Label>
          )
        })}
      </RadioGroup>
    </div>
  )
}
