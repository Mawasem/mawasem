import { z } from "zod";

export const blockCustomerDefaultValues = {
  reason: "",
};

export type BlockCustomerFormValues =
  typeof blockCustomerDefaultValues;

export const createBlockCustomerFormSchema = (
  t: (key: string) => string
) =>
  z.object({
    reason: z
      .string()
      .trim()
      .min(2, t("customers.validation.reasonMin")),
  });
