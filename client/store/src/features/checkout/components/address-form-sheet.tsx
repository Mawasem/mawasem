import { zodResolver } from "@hookform/resolvers/zod"
import { LoaderCircle, Plus } from "lucide-react"
import { useMemo, useState } from "react"
import {
  useForm,
  useWatch,
  type FieldError as HookFormFieldError,
  type UseFormRegisterReturn,
} from "react-hook-form"

import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Field,
  FieldError,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet"
import { getApiErrorMessage } from "@/lib/get-api-error-message"

import { useCreateCustomerAddress } from "../hooks/use-create-customer-address"
import { useDeliveryAreas } from "../hooks/use-delivery-areas"
import {
  getCheckoutCopy,
  getCheckoutLocale,
} from "../i18n/checkout-copy"
import {
  createAddressSchema,
  type AddressFormValues,
} from "../schemas/address-schema"
import type { CustomerAddress } from "../types/checkout.types"

interface AddressFormSheetProps {
  onCreated: (address: CustomerAddress) => void
}

export function AddressFormSheet({ onCreated }: AddressFormSheetProps) {
  const [open, setOpen] = useState(false)
  const copy = getCheckoutCopy()
  const locale = getCheckoutLocale()
  const createAddressMutation = useCreateCustomerAddress()
  const { deliveryAreasData, isLoading: isLoadingDeliveryAreas } =
    useDeliveryAreas()
  const addressSchema = useMemo(
    () => createAddressSchema(copy.address.validation),
    [copy.address.validation]
  )

  const form = useForm<AddressFormValues>({
    resolver: zodResolver(addressSchema),
    defaultValues: {
      label: copy.address.defaultLabel,
      city: "",
      areaName: "",
      detailedAddress: "",
      buildingNumber: "",
      floorNumber: "",
      apartmentNumber: "",
      landmark: "",
      recipientName: "",
      recipientPhone: "",
      deliveryAreaId: 0,
      isDefault: false,
    },
  })

  async function onSubmit(values: AddressFormValues) {
    try {
      const address = await createAddressMutation.createAddressAsync({
        ...values,
        buildingNumber: values.buildingNumber || null,
        floorNumber: values.floorNumber || null,
        apartmentNumber: values.apartmentNumber || null,
        landmark: values.landmark || null,
        customDeliveryAreaNameAr: null,
        customDeliveryAreaNameEn: null,
      })

      onCreated(address)
      form.reset()
      setOpen(false)
    } catch {
      // Mutation error is rendered below.
    }
  }

  const selectedDeliveryAreaId = useWatch({
    control: form.control,
    name: "deliveryAreaId",
  })
  const isDefault = useWatch({
    control: form.control,
    name: "isDefault",
  })

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button type="button" variant="outline">
          <Plus className="size-4" />
          {copy.address.add}
        </Button>
      </SheetTrigger>

      <SheetContent className="w-full overflow-y-auto sm:max-w-xl">
        <SheetHeader>
          <SheetTitle>{copy.address.addTitle}</SheetTitle>
          <SheetDescription>
            {copy.address.addDescription}
          </SheetDescription>
        </SheetHeader>

        <form
          className="space-y-5 px-4 pb-6"
          onSubmit={form.handleSubmit(onSubmit)}
        >
          <FieldGroup>
            <div className="grid gap-4 sm:grid-cols-2">
              <TextField
                id="label"
                label={copy.address.fields.label}
                error={form.formState.errors.label}
                inputProps={form.register("label")}
              />
              <TextField
                id="city"
                label={copy.address.fields.city}
                error={form.formState.errors.city}
                inputProps={form.register("city")}
              />
              <TextField
                id="areaName"
                label={copy.address.fields.area}
                error={form.formState.errors.areaName}
                inputProps={form.register("areaName")}
              />
              <TextField
                id="recipientName"
                label={copy.address.fields.recipientName}
                error={form.formState.errors.recipientName}
                inputProps={form.register("recipientName")}
              />
              <TextField
                id="recipientPhone"
                label={copy.address.fields.recipientPhone}
                type="tel"
                error={form.formState.errors.recipientPhone}
                inputProps={form.register("recipientPhone")}
              />
              <TextField
                id="buildingNumber"
                label={copy.address.fields.building}
                error={form.formState.errors.buildingNumber}
                inputProps={form.register("buildingNumber")}
              />
              <TextField
                id="floorNumber"
                label={copy.address.fields.floor}
                error={form.formState.errors.floorNumber}
                inputProps={form.register("floorNumber")}
              />
              <TextField
                id="apartmentNumber"
                label={copy.address.fields.apartment}
                error={form.formState.errors.apartmentNumber}
                inputProps={form.register("apartmentNumber")}
              />
            </div>

            <TextField
              id="detailedAddress"
              label={copy.address.fields.detailedAddress}
              error={form.formState.errors.detailedAddress}
              inputProps={form.register("detailedAddress")}
            />

            <TextField
              id="landmark"
              label={copy.address.fields.landmark}
              error={form.formState.errors.landmark}
              inputProps={form.register("landmark")}
            />

            <Field>
              <FieldLabel>{copy.address.fields.deliveryArea}</FieldLabel>
              <Select
                value={
                  selectedDeliveryAreaId
                    ? String(selectedDeliveryAreaId)
                    : undefined
                }
                onValueChange={(value) =>
                  form.setValue("deliveryAreaId", Number(value), {
                    shouldValidate: true,
                  })
                }
                disabled={isLoadingDeliveryAreas}
              >
                <SelectTrigger>
                  <SelectValue
                    placeholder={copy.address.fields.selectDeliveryArea}
                  />
                </SelectTrigger>
                <SelectContent>
                  {deliveryAreasData?.items.map((area) => (
                    <SelectItem key={area.id} value={String(area.id)}>
                      {locale === "ar" ? area.nameAr : area.nameEn} —{" "}
                      {area.isFreeDelivery
                        ? copy.summary.free
                        : `EGP ${area.deliveryFee.toFixed(2)}`}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FieldError errors={[form.formState.errors.deliveryAreaId]} />
            </Field>

            <label className="flex items-center gap-2 text-sm">
              <Checkbox
                checked={isDefault}
                onCheckedChange={(checked) =>
                  form.setValue("isDefault", checked === true)
                }
              />
              {copy.address.fields.setDefault}
            </label>

            {createAddressMutation.error ? (
              <p className="text-sm text-destructive">
                {getApiErrorMessage(
                  createAddressMutation.error,
                  copy.address.createFailed
                )}
              </p>
            ) : null}

            <Button type="submit" disabled={createAddressMutation.isLoading}>
              {createAddressMutation.isLoading ? (
                <>
                  <LoaderCircle className="size-4 animate-spin" />
                  {copy.address.saving}
                </>
              ) : (
                copy.address.save
              )}
            </Button>
          </FieldGroup>
        </form>
      </SheetContent>
    </Sheet>
  )
}

interface TextFieldProps {
  id: string
  label: string
  type?: "text" | "tel"
  error?: HookFormFieldError
  inputProps: UseFormRegisterReturn
}

function TextField({
  id,
  label,
  type = "text",
  error,
  inputProps,
}: TextFieldProps) {
  return (
    <Field>
      <FieldLabel htmlFor={id}>{label}</FieldLabel>
      <Input id={id} type={type} {...inputProps} />
      <FieldError errors={[error]} />
    </Field>
  )
}
