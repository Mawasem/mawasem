import type { TFunction } from "i18next";
import { z } from "zod";

const requiredText = (t: TFunction, key: string, max: number) =>
  z
    .string()
    .trim()
    .min(1, t("products.validation.required"))
    .max(max, t(key, { max }));

export const createProductFormSchema = (t: TFunction) => {
  const specificationSchema = z.object({
    nameAr: requiredText(t, "products.validation.maxLength", 100),
    nameEn: requiredText(t, "products.validation.maxLength", 100),
    valueAr: requiredText(t, "products.validation.maxLength", 500),
    valueEn: requiredText(t, "products.validation.maxLength", 500),
  });

  return z
    .object({
      nameAr: requiredText(t, "products.validation.maxLength", 200),
      nameEn: requiredText(t, "products.validation.maxLength", 200),
      descriptionAr: requiredText(t, "products.validation.maxLength", 2000),
      descriptionEn: requiredText(t, "products.validation.maxLength", 2000),
      originalPrice: z
        .number({ error: t("products.validation.invalidNumber") })
        .positive(t("products.validation.positivePrice")),
      currentPrice: z
        .number({ error: t("products.validation.invalidNumber") })
        .positive(t("products.validation.positivePrice")),
      slug: z
        .string()
        .trim()
        .min(1, t("products.validation.required"))
        .max(300, t("products.validation.maxLength", { max: 300 }))
        .regex(
          /^[a-z0-9]+(?:-[a-z0-9]+)*$/,
          t("products.validation.invalidSlug")
        ),
      brandId: z.number().int().positive(t("products.validation.selectBrand")),
      seasonId: z.number().int().positive(t("products.validation.selectSeason")),
      categoryIds: z
        .array(z.number().int().positive())
        .min(1, t("products.validation.selectCategory")),
      collectionIds: z.array(z.number().int().positive()),
      specifications: z
        .array(specificationSchema)
        .max(50, t("products.validation.maxSpecifications")),
    })
    .refine((values) => values.currentPrice <= values.originalPrice, {
      path: ["currentPrice"],
      message: t("products.validation.currentPriceTooHigh"),
    })
    .superRefine((values, context) => {
      const normalizedArabicNames = values.specifications.map((item) =>
        item.nameAr.trim().toLocaleUpperCase()
      );
      const normalizedEnglishNames = values.specifications.map((item) =>
        item.nameEn.trim().toLocaleUpperCase()
      );

      if (new Set(normalizedArabicNames).size !== normalizedArabicNames.length) {
        context.addIssue({
          code: "custom",
          path: ["specifications"],
          message: t("products.validation.duplicateArabicSpecification"),
        });
      }

      if (new Set(normalizedEnglishNames).size !== normalizedEnglishNames.length) {
        context.addIssue({
          code: "custom",
          path: ["specifications"],
          message: t("products.validation.duplicateEnglishSpecification"),
        });
      }
    });
};

export type ProductFormValues = z.infer<
  ReturnType<typeof createProductFormSchema>
>;

export const productFormDefaultValues: ProductFormValues = {
  nameAr: "",
  nameEn: "",
  descriptionAr: "",
  descriptionEn: "",
  originalPrice: 1,
  currentPrice: 1,
  slug: "",
  brandId: 0,
  seasonId: 0,
  categoryIds: [],
  collectionIds: [],
  specifications: [],
};
