import { useEffect } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { useTranslation } from "react-i18next";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";

import { useSeasons } from "@/features/seasons/hooks/use-seasons";
import { CATALOGUE_OPTIONS_PAGE_SIZE } from "@/lib/catalogue-options";
import {
  collectionFormDefaultValues,
  createCollectionFormSchema,
  type CollectionFormValues,
} from "../schema/collection-form-schema";
import type { CollectionFormProps } from "./types";

export function CollectionForm({
  mode,
  collection,
  formId,
  errorMessage,
  onSubmit,
}: CollectionFormProps) {
  const { t, i18n } = useTranslation();

  const collectionFormSchema =
    createCollectionFormSchema(t);

  const form = useForm<CollectionFormValues>({
    resolver: zodResolver(collectionFormSchema),
    defaultValues:
      mode === "edit" && collection
        ? {
          nameAr: collection.nameAr,
          nameEn: collection.nameEn,
          seasonId: collection.seasonId,
        }
        : collectionFormDefaultValues,
  });

  useEffect(() => {
    if (mode === "edit" && collection) {
      form.reset({
        nameAr: collection.nameAr,
        nameEn: collection.nameEn,
        seasonId: collection.seasonId,
      });

      return;
    }

    form.reset(collectionFormDefaultValues);
  }, [collection, form, mode]);

  const { data: seasonsData, isLoading: isSeasonsLoading } =
    useSeasons({
      includeDeleted: false,
      pageNumber: 1,
      pageSize: CATALOGUE_OPTIONS_PAGE_SIZE,
    });

  const handleFormSubmit = async (
    values: CollectionFormValues
  ) => {
    await onSubmit(values);
  };

  return (
    <Form {...form}>
      <form
        id={formId}
        onSubmit={form.handleSubmit(handleFormSubmit)}
        className="space-y-5"
      >
        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="nameAr"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{t("collections.form.nameAr")}</FormLabel>
                <FormControl>
                  <Input
                    placeholder={t("collections.form.nameAr")}
                    {...field}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          <FormField
            control={form.control}
            name="nameEn"
            render={({ field }) => (
              <FormItem>
                <FormLabel>{t("collections.form.nameEn")}</FormLabel>
                <FormControl>
                  <Input
                    placeholder={t("collections.form.nameEn")}
                    {...field}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <FormField
          control={form.control}
          name="seasonId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>{t("collections.form.season")}</FormLabel>
              <FormControl>
                <select
                  value={field.value || ""}
                  onChange={(event) => {
                    const nextValue = event.target.value;

                    field.onChange(
                      nextValue
                        ? Number(nextValue)
                        : 0
                    );
                  }}
                  className="h-9 w-full min-w-0 rounded-4xl border border-input bg-input/30 px-3 py-1 text-base transition-colors outline-none placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 disabled:pointer-events-none disabled:cursor-not-allowed disabled:opacity-50 aria-invalid:border-destructive aria-invalid:ring-[3px] aria-invalid:ring-destructive/20 md:text-sm dark:aria-invalid:border-destructive/50 dark:aria-invalid:ring-destructive/40"
                  disabled={isSeasonsLoading}
                >
                  <option value="">
                    {t("collections.form.selectSeason")}
                  </option>

                  {(seasonsData?.items ?? []).map(
                    (season) => (
                      <option
                        key={season.id}
                        value={season.id}
                      >
                        {i18n.resolvedLanguage === "ar"
                          ? season.nameAr
                          : season.nameEn}
                      </option>
                    )
                  )}
                </select>
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {errorMessage ? (
          <p className="text-sm text-destructive">
            {errorMessage}
          </p>
        ) : null}
      </form>
    </Form>
  );
}
