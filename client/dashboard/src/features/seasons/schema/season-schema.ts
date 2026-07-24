import { z } from "zod";

export const createSeasonFormSchema = (
  t: (key: string) => string
) =>
  z.object({
    nameAr: z
      .string()
      .trim()
      .min(1, t("seasons.validation.nameArRequired")),
    nameEn: z
      .string()
      .trim()
      .min(1, t("seasons.validation.nameEnRequired")),
    descriptionAr: z
      .string()
      .trim()
      .min(1, t("seasons.validation.descriptionArRequired")),
    descriptionEn: z
      .string()
      .trim()
      .min(1, t("seasons.validation.descriptionEnRequired")),
    isActive: z.boolean(),
  });

export type SeasonFormValues = z.infer<
  ReturnType<typeof createSeasonFormSchema>
>;

export const seasonFormDefaultValues: SeasonFormValues = {
  nameAr: "",
  nameEn: "",
  descriptionAr: "",
  descriptionEn: "",
  isActive: true,
};
