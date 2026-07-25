import { z } from "zod";

export const createCollectionFormSchema = (
  t: (key: string) => string
) =>
  z.object({
    nameAr: z
      .string()
      .trim()
      .min(1, t("collections.validation.nameAr")),
    nameEn: z
      .string()
      .trim()
      .min(1, t("collections.validation.nameEn")),
    seasonId: z
      .number()
      .min(1, t("collections.validation.season")),
  });

export type CollectionFormValues =
  z.infer<
    ReturnType<typeof createCollectionFormSchema>
  >;

export const collectionFormDefaultValues: CollectionFormValues =
  {
    nameAr: "",
    nameEn: "",
    seasonId: 0,
  };
