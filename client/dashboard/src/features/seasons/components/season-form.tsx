import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";

import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";
import { Textarea } from "@/components/ui/textarea";
import {
  seasonFormDefaultValues,
  seasonFormSchema,
  type SeasonFormValues,
} from "../schema/season-schema";
import type { SeasonFormProps } from "../types";

export function SeasonForm({
  mode,
  season,
  formId,
  onSubmit,
  errorMessage,
}: SeasonFormProps) {
  const form = useForm<SeasonFormValues>({
    resolver: zodResolver(seasonFormSchema),
    defaultValues:
      mode === "edit" && season
        ? {
          nameAr: season.nameAr,
          nameEn: season.nameEn,
          descriptionAr: season.descriptionAr,
          descriptionEn: season.descriptionEn,
          isActive: season.isActive,
        }
        : seasonFormDefaultValues,
  });

  useEffect(() => {
    if (mode === "edit" && season) {
      form.reset({
        nameAr: season.nameAr,
        nameEn: season.nameEn,
        descriptionAr: season.descriptionAr,
        descriptionEn: season.descriptionEn,
        isActive: season.isActive,
      });

      return;
    }

    form.reset(seasonFormDefaultValues);
  }, [season, form, mode]);

  const handleFormSubmit = async (
    values: SeasonFormValues
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
                <FormLabel>Arabic Name</FormLabel>
                <FormControl>
                  <Input placeholder="Season name in Arabic" {...field} />
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
                <FormLabel>English Name</FormLabel>
                <FormControl>
                  <Input placeholder="Season name in English" {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        <FormField
          control={form.control}
          name="descriptionAr"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Arabic Description</FormLabel>
              <FormControl>
                <Textarea
                  placeholder="Arabic season description"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="descriptionEn"
          render={({ field }) => (
            <FormItem>
              <FormLabel>English Description</FormLabel>
              <FormControl>
                <Textarea
                  placeholder="English season description"
                  {...field}
                />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="isActive"
          render={({ field }) => (
            <FormItem className="flex flex-row items-center justify-between rounded-2xl border p-4">
              <div className="space-y-0.5">
                <FormLabel>Active</FormLabel>
                <p className="text-sm text-muted-foreground">
                  Toggle to control whether the season is visible.
                </p>
              </div>

              <FormControl>
                <Switch
                  checked={field.value}
                  onCheckedChange={field.onChange}
                />
              </FormControl>
            </FormItem>
          )}
        />

        {errorMessage ? (
          <p className="text-sm text-destructive">{errorMessage}</p>
        ) : null}
      </form>
    </Form>
  );
}
