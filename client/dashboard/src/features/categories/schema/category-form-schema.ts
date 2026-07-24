import { z } from "zod";

export const createCategoryFormSchema = (
  t: (key: string) => string
) =>
  z.object({
    nameAr: z
      .string()
      .trim()
      .min(2, t("categories.validation.nameArMin")),
    nameEn: z
      .string()
      .trim()
      .min(2, t("categories.validation.nameEnMin")),
  });

export type CategoryFormValues =
  z.infer<
    ReturnType<typeof createCategoryFormSchema>
  >;

export const categoryFormDefaultValues: CategoryFormValues =
{
  nameAr: "",
  nameEn: "",
};
