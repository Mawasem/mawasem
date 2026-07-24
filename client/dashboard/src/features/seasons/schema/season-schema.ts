import { z } from "zod";

export const seasonFormSchema = z.object({
  nameAr: z
    .string()
    .trim()
    .min(1, "Arabic name is required."),
  nameEn: z
    .string()
    .trim()
    .min(1, "English name is required."),
  descriptionAr: z
    .string()
    .trim()
    .min(1, "Arabic description is required."),
  descriptionEn: z
    .string()
    .trim()
    .min(1, "English description is required."),
  isActive: z.boolean(),
});

export type SeasonFormValues = z.infer<typeof seasonFormSchema>;

export const seasonFormDefaultValues: SeasonFormValues = {
  nameAr: "",
  nameEn: "",
  descriptionAr: "",
  descriptionEn: "",
  isActive: true,
};
