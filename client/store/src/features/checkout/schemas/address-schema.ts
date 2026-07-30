import { z } from "zod"

interface AddressValidationMessages {
  labelRequired: string
  cityRequired: string
  areaRequired: string
  detailedAddressRequired: string
  recipientNameRequired: string
  recipientPhoneInvalid: string
  deliveryAreaRequired: string
}

export function createAddressSchema(messages: AddressValidationMessages) {
  return z.object({
    label: z.string().trim().min(2, messages.labelRequired),
    city: z.string().trim().min(2, messages.cityRequired),
    areaName: z.string().trim().min(2, messages.areaRequired),
    detailedAddress: z
      .string()
      .trim()
      .min(5, messages.detailedAddressRequired),
    buildingNumber: z.string().trim().optional(),
    floorNumber: z.string().trim().optional(),
    apartmentNumber: z.string().trim().optional(),
    landmark: z.string().trim().optional(),
    recipientName: z.string().trim().min(2, messages.recipientNameRequired),
    recipientPhone: z
      .string()
      .trim()
      .regex(/^01[0125][0-9]{8}$/, messages.recipientPhoneInvalid),
    deliveryAreaId: z
      .number()
      .int()
      .positive(messages.deliveryAreaRequired),
    isDefault: z.boolean(),
  })
}

export type AddressFormValues = z.infer<
  ReturnType<typeof createAddressSchema>
>
