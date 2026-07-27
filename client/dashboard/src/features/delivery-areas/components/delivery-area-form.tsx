import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, useWatch } from "react-hook-form";
import { useTranslation } from "react-i18next";

import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { Switch } from "@/components/ui/switch";

import {
  createDeliveryAreaFormSchema,
  deliveryAreaFormDefaultValues,
  type DeliveryAreaFormValues,
} from "../schema/delivery-area-form-schema";
import {
  DeliveryAreaStatus,
  type DeliveryAreaFormProps,
} from "../types";

const selectClassName =
  "h-9 w-full rounded-4xl border border-input bg-input/30 px-3 text-sm outline-none transition-colors focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50 disabled:cursor-not-allowed disabled:opacity-50";

export function DeliveryAreaForm({
  mode,
  deliveryArea,
  formId,
  errorMessage,
  onSubmit,
}: DeliveryAreaFormProps) {
  const { t } = useTranslation();

  const form = useForm<DeliveryAreaFormValues>({
    resolver: zodResolver(createDeliveryAreaFormSchema(t)),
    defaultValues:
      mode === "edit" && deliveryArea
        ? {
            nameAr: deliveryArea.nameAr,
            nameEn: deliveryArea.nameEn,
            deliveryFee: deliveryArea.deliveryFee,
            isFreeDelivery: deliveryArea.isFreeDelivery,
            isActive: deliveryArea.isActive,
            status: deliveryArea.status,
          }
        : deliveryAreaFormDefaultValues,
  });

  const isFreeDelivery = useWatch({
    control: form.control,
    name: "isFreeDelivery",
  });

  return (
    <Form {...form}>
      <form
        id={formId}
        onSubmit={form.handleSubmit(onSubmit)}
        className="space-y-5"
      >
        <div className="grid gap-4 md:grid-cols-2">
          <FormField
            control={form.control}
            name="nameAr"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  {t("deliveryAreas.form.nameArLabel")}
                </FormLabel>
                <FormControl>
                  <Input
                    dir="rtl"
                    placeholder={t(
                      "deliveryAreas.form.nameArPlaceholder"
                    )}
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
                <FormLabel>
                  {t("deliveryAreas.form.nameEnLabel")}
                </FormLabel>
                <FormControl>
                  <Input
                    dir="ltr"
                    placeholder={t(
                      "deliveryAreas.form.nameEnPlaceholder"
                    )}
                    {...field}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        {mode === "create" ? (
          <FormField
            control={form.control}
            name="status"
            render={({ field }) => (
              <FormItem>
                <FormLabel>
                  {t("deliveryAreas.form.statusLabel")}
                </FormLabel>
                <FormControl>
                  <select
                    className={selectClassName}
                    value={field.value}
                    onChange={(event) =>
                      field.onChange(Number(event.target.value))
                    }
                  >
                    <option value={DeliveryAreaStatus.Pending}>
                      {t("deliveryAreas.status.pending")}
                    </option>
                    <option value={DeliveryAreaStatus.Confirmed}>
                      {t("deliveryAreas.status.confirmed")}
                    </option>
                    <option value={DeliveryAreaStatus.Restricted}>
                      {t("deliveryAreas.status.restricted")}
                    </option>
                  </select>
                </FormControl>
                <FormDescription>
                  {t("deliveryAreas.form.statusHint")}
                </FormDescription>
                <FormMessage />
              </FormItem>
            )}
          />
        ) : null}

        <FormField
          control={form.control}
          name="deliveryFee"
          render={({ field }) => (
            <FormItem>
              <FormLabel>
                {t("deliveryAreas.form.deliveryFeeLabel")}
              </FormLabel>
              <FormControl>
                <Input
                  type="number"
                  inputMode="decimal"
                  min={0}
                  step="0.01"
                  disabled={isFreeDelivery}
                  value={Number.isNaN(field.value) ? "" : field.value}
                  onBlur={field.onBlur}
                  onChange={(event) =>
                    field.onChange(
                      event.target.value === ""
                        ? Number.NaN
                        : event.target.valueAsNumber
                    )
                  }
                  name={field.name}
                  ref={field.ref}
                />
              </FormControl>
              <FormDescription>
                {isFreeDelivery
                  ? t("deliveryAreas.form.freeDeliveryFeeHint")
                  : t("deliveryAreas.form.deliveryFeeHint")}
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="isFreeDelivery"
          render={({ field }) => (
            <FormItem className="flex items-center justify-between gap-4 rounded-2xl border p-4">
              <div className="space-y-1">
                <FormLabel>
                  {t("deliveryAreas.form.freeDeliveryLabel")}
                </FormLabel>
                <FormDescription>
                  {t("deliveryAreas.form.freeDeliveryHint")}
                </FormDescription>
              </div>

              <FormControl>
                <Switch
                  checked={field.value}
                  onCheckedChange={(checked) => {
                    field.onChange(checked);

                    if (checked) {
                      form.setValue("deliveryFee", 0, {
                        shouldDirty: true,
                        shouldValidate: true,
                      });
                    }
                  }}
                />
              </FormControl>
            </FormItem>
          )}
        />

        <FormField
          control={form.control}
          name="isActive"
          render={({ field }) => (
            <FormItem className="flex items-center justify-between gap-4 rounded-2xl border p-4">
              <div className="space-y-1">
                <FormLabel>
                  {t("deliveryAreas.form.activeLabel")}
                </FormLabel>
                <FormDescription>
                  {t("deliveryAreas.form.activeHint")}
                </FormDescription>
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
