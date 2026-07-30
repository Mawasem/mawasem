import { CircleAlert, LoaderCircle } from "lucide-react"
import { useMemo, useState } from "react"
import { useNavigate } from "react-router-dom"

import {
  Alert,
  AlertDescription,
  AlertTitle,
} from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Skeleton } from "@/components/ui/skeleton"
import { useCart } from "@/features/cart/hooks/use-cart"
import { getApiErrorMessage } from "@/lib/get-api-error-message"

import { AddressCard } from "../components/address-card"
import { AddressFormSheet } from "../components/address-form-sheet"
import { CheckoutSummary } from "../components/checkout-summary"
import { DeliveryMethodSelector } from "../components/delivery-method-selector"
import { useCheckoutPreview } from "../hooks/use-checkout-preview"
import { useCustomerAddresses } from "../hooks/use-customer-addresses"
import { usePlaceOrder } from "../hooks/use-place-order"
import { getCheckoutCopy } from "../i18n/checkout-copy"
import {
  DeliveryMethod,
  PaymentMethod,
  type CustomerAddress,
  type DeliveryMethodValue,
} from "../types/checkout.types"

export default function CheckoutPage() {
  const copy = getCheckoutCopy()
  const navigate = useNavigate()
  const [deliveryMethod, setDeliveryMethod] =
    useState<DeliveryMethodValue>(DeliveryMethod.HomeDelivery)
  const [selectedAddressId, setSelectedAddressId] = useState<number | null>(
    null
  )
  const [notes, setNotes] = useState("")
  const paymentMethod = PaymentMethod.CashOnDelivery

  const {
    cartData,
    isLoading: cartLoading,
    error: cartError,
  } = useCart()
  const {
    addressesData,
    isLoading: addressesLoading,
    error: addressesError,
  } = useCustomerAddresses()

  const defaultAddress =
    addressesData?.items.find((address) => address.isDefault) ??
    addressesData?.items[0] ??
    null
  const selectedAddress =
    addressesData?.items.find(
      (address) => address.id === selectedAddressId
    ) ??
    defaultAddress ??
    null
  const isHomeDelivery =
    deliveryMethod === DeliveryMethod.HomeDelivery
  const previewAddressId =
    isHomeDelivery && selectedAddress ? selectedAddress.id : null

  const {
    previewData,
    isLoading: previewLoading,
    error: previewError,
  } = useCheckoutPreview(
    cartData?.cartId ?? null,
    deliveryMethod,
    previewAddressId,
    paymentMethod
  )
  const placeOrderMutation = usePlaceOrder()
  const idempotencyKey = useMemo(() => crypto.randomUUID(), [])

  function handleDeliveryMethodChange(nextValue: DeliveryMethodValue) {
    setDeliveryMethod(nextValue)
    placeOrderMutation.reset()
  }

  async function handlePlaceOrder() {
    const orderAddressId =
      isHomeDelivery && selectedAddress ? selectedAddress.id : null

    if (
      !previewData ||
      previewData.deliveryMethod !== deliveryMethod ||
      (isHomeDelivery && orderAddressId === null)
    ) {
      return
    }

    try {
      const order = await placeOrderMutation.placeOrderAsync({
        deliveryMethod,
        userAddressId: orderAddressId,
        paymentMethod,
        notes: notes.trim() || null,
        idempotencyKey,
      })

      navigate(`/checkout/success/${order.orderNumber}`, {
        replace: true,
        state: { order },
      })
    } catch {
      // Mutation error is rendered below.
    }
  }

  function handleCreated(address: CustomerAddress) {
    setSelectedAddressId(address.id)
  }

  const previewMatchesSelection =
    previewData !== undefined &&
    previewData.deliveryMethod === deliveryMethod &&
    previewData.userAddressId === previewAddressId

  return (
    <section className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">
          {copy.page.title}
        </h1>
        <p className="text-muted-foreground">{copy.page.description}</p>
      </div>

      <Card>
        <CardContent>
          <DeliveryMethodSelector
            value={deliveryMethod}
            onValueChange={handleDeliveryMethodChange}
            disabled={placeOrderMutation.isLoading}
          />
        </CardContent>
      </Card>

      {cartError ? (
        <CheckoutAlert
          title={copy.errors.cartFailed}
          description={getApiErrorMessage(
            cartError,
            copy.errors.cartFailed
          )}
        />
      ) : null}

      <div className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_24rem]">
        <div className="space-y-6">
          {isHomeDelivery ? (
            addressesLoading ? (
              <AddressSectionSkeleton />
            ) : (
              <Card>
                <CardHeader className="flex-row items-center justify-between">
                  <CardTitle>{copy.address.title}</CardTitle>
                  <AddressFormSheet onCreated={handleCreated} />
                </CardHeader>
                <CardContent className="space-y-3">
                  {addressesError ? (
                    <CheckoutAlert
                      title={copy.errors.addressesFailed}
                      description={getApiErrorMessage(
                        addressesError,
                        copy.errors.addressesFailed
                      )}
                    />
                  ) : null}

                  {addressesData?.items.length ? (
                    addressesData.items.map((address) => (
                      <AddressCard
                        key={address.id}
                        address={address}
                        selected={address.id === selectedAddress?.id}
                        onSelect={() => {
                          setSelectedAddressId(address.id)
                          placeOrderMutation.reset()
                        }}
                      />
                    ))
                  ) : (
                    <p className="rounded-lg border border-dashed p-6 text-center text-muted-foreground">
                      {copy.address.empty}
                    </p>
                  )}

                  {!selectedAddress ? (
                    <CheckoutAlert
                      title={copy.address.title}
                      description={copy.address.required}
                    />
                  ) : null}
                </CardContent>
              </Card>
            )
          ) : null}

          <Card>
            <CardHeader>
              <CardTitle>{copy.payment.title}</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label>{copy.payment.method}</Label>
                <Select value={String(paymentMethod)} disabled>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value={String(PaymentMethod.CashOnDelivery)}>
                      {copy.payment.cashOnDelivery}
                    </SelectItem>
                  </SelectContent>
                </Select>
                <p className="text-sm text-muted-foreground">
                  {copy.payment.onlineUnavailable}
                </p>
              </div>

              <div className="space-y-2">
                <Label htmlFor="notes">{copy.payment.notes}</Label>
                <Input
                  id="notes"
                  value={notes}
                  onChange={(event) => setNotes(event.target.value)}
                  placeholder={copy.payment.notesPlaceholder}
                  maxLength={500}
                  disabled={placeOrderMutation.isLoading}
                />
              </div>
            </CardContent>
          </Card>

          {previewError ? (
            <CheckoutAlert
              title={copy.errors.previewFailed}
              description={getApiErrorMessage(
                previewError,
                copy.errors.previewFailed
              )}
            />
          ) : null}

          {placeOrderMutation.error ? (
            <CheckoutAlert
              title={copy.errors.placeOrderFailed}
              description={getApiErrorMessage(
                placeOrderMutation.error,
                copy.errors.placeOrderFailed
              )}
            />
          ) : null}

          <Button
            size="lg"
            className="w-full"
            disabled={
              !previewData?.canPlaceOrder ||
              !previewMatchesSelection ||
              placeOrderMutation.isLoading ||
              previewLoading ||
              cartLoading ||
              (isHomeDelivery && !selectedAddress)
            }
            onClick={() => void handlePlaceOrder()}
          >
            {placeOrderMutation.isLoading ? (
              <>
                <LoaderCircle className="size-4 animate-spin" />
                {copy.actions.placingOrder}
              </>
            ) : (
              copy.actions.placeOrder
            )}
          </Button>
        </div>

        <div>
          {cartLoading || previewLoading ? (
            <SummarySkeleton />
          ) : previewData && previewMatchesSelection ? (
            <CheckoutSummary
              preview={previewData}
              selectedAddress={isHomeDelivery ? selectedAddress : null}
            />
          ) : null}
        </div>
      </div>
    </section>
  )
}

function CheckoutAlert({
  title,
  description,
}: {
  title: string
  description: string
}) {
  return (
    <Alert className="border-destructive/30 bg-destructive/5 text-destructive">
      <CircleAlert />
      <AlertTitle>{title}</AlertTitle>
      <AlertDescription className="text-destructive/90">
        {description}
      </AlertDescription>
    </Alert>
  )
}

function AddressSectionSkeleton() {
  return (
    <Card>
      <CardHeader>
        <Skeleton className="h-6 w-40" />
      </CardHeader>
      <CardContent className="space-y-3">
        <Skeleton className="h-28 w-full" />
        <Skeleton className="h-28 w-full" />
      </CardContent>
    </Card>
  )
}

function SummarySkeleton() {
  return (
    <Card className="h-fit lg:sticky lg:top-24">
      <CardHeader>
        <Skeleton className="h-6 w-36" />
      </CardHeader>
      <CardContent className="space-y-4">
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-16 w-full" />
        <Skeleton className="h-px w-full" />
        <Skeleton className="h-24 w-full" />
      </CardContent>
    </Card>
  )
}
