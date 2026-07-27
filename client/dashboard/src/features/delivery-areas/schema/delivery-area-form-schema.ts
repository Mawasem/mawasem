import { z } from "zod";

import { DeliveryAreaStatus } from "../types";

export const createDeliveryAreaFormSchema = (
  t: (key: string) => string
) =>
  z.object({
    nameAr: z
      .string()
      .trim()
      .min(1, t("deliveryAreas.validation.nameArRequired"))
      .max(200, t("deliveryAreas.validation.nameMaxLength")),
    nameEn: z
      .string()
      .trim()
      .min(1, t("deliveryAreas.validation.nameEnRequired"))
      .max(200, t("deliveryAreas.validation.nameMaxLength")),
    deliveryFee: z
      .number({
        error: t("deliveryAreas.validation.deliveryFeeRequired"),
      })
      .min(0, t("deliveryAreas.validation.deliveryFeeNonNegative")),
    isFreeDelivery: z.boolean(),
    isActive: z.boolean(),
    status: z.union([
      z.literal(DeliveryAreaStatus.Pending),
      z.literal(DeliveryAreaStatus.Confirmed),
      z.literal(DeliveryAreaStatus.Restricted),
    ]),
  });

export type DeliveryAreaFormValues = z.infer<
  ReturnType<typeof createDeliveryAreaFormSchema>
>;

export const deliveryAreaFormDefaultValues: DeliveryAreaFormValues = {
  nameAr: "",
  nameEn: "",
  deliveryFee: 0,
  isFreeDelivery: false,
  isActive: true,
  status: DeliveryAreaStatus.Confirmed,
};
